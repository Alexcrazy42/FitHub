import React, { useEffect, useState, useCallback } from 'react';
import { Modal, Button, Spin, Alert, Space } from 'antd';
import {
  DownloadOutlined,
  ZoomInOutlined,
  ZoomOutOutlined,
} from '@ant-design/icons';
import * as XLSX from 'xlsx';
import { useApiService } from '../../../../api/useApiService';

// mammoth doesn't ship ESM types; import via require-style interop
// eslint-disable-next-line @typescript-eslint/no-explicit-any
let mammoth: any = null;
import('mammoth').then((m) => { mammoth = m.default ?? m; });

interface DocumentViewerModalProps {
  fileId: string | null;
  fileName: string;
  mimeType: string;
  onClose: () => void;
}

type DocType = 'pdf' | 'excel' | 'word' | 'other';

function getDocType(mimeType: string, fileName: string): DocType {
  const lower = fileName.toLowerCase();
  if (mimeType === 'application/pdf' || lower.endsWith('.pdf')) return 'pdf';
  if (
    mimeType.includes('spreadsheetml') ||
    mimeType.includes('ms-excel') ||
    lower.endsWith('.xlsx') ||
    lower.endsWith('.xls') ||
    lower.endsWith('.csv')
  ) return 'excel';
  if (
    mimeType.includes('wordprocessingml') ||
    mimeType.includes('msword') ||
    lower.endsWith('.docx') ||
    lower.endsWith('.doc')
  ) return 'word';
  return 'other';
}

const DocumentViewerModal: React.FC<DocumentViewerModalProps> = ({
  fileId,
  fileName,
  mimeType,
  onClose,
}) => {
  const apiService = useApiService();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [objectUrl, setObjectUrl] = useState<string | null>(null);
  const [htmlContent, setHtmlContent] = useState<string | null>(null);
  const [zoom, setZoom] = useState(1);

  const docType = getDocType(mimeType, fileName);

  const fetchAndRender = useCallback(async () => {
    if (!fileId) return;
    setLoading(true);
    setError(null);

    // cleanup previous
    setObjectUrl((prev) => { if (prev) URL.revokeObjectURL(prev); return null; });
    setHtmlContent(null);

    try {
      const blob = await apiService.getBlob(`/v1/files/${fileId}`);

      if (!blob) {
        setError('Не удалось загрузить файл');
        return;
      }

      if (docType === 'pdf') {
        const url = URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));
        setObjectUrl(url);
      } else if (docType === 'excel') {
        const buffer = await blob.arrayBuffer();
        const workbook = XLSX.read(new Uint8Array(buffer), { type: 'array' });
        const firstSheet = workbook.Sheets[workbook.SheetNames[0]];
        const rawHtml = XLSX.utils.sheet_to_html(firstSheet, { id: 'xlsx-table', editable: false });
        // Inject basic table styles so the sheet is readable
        const styledHtml = rawHtml.replace(
          '<table',
          `<style>
            #xlsx-table { border-collapse: collapse; font-size: 13px; font-family: sans-serif; }
            #xlsx-table td, #xlsx-table th { border: 1px solid #d0d0d0; padding: 4px 8px; white-space: nowrap; }
            #xlsx-table tr:nth-child(even) { background: #f7f7f7; }
            #xlsx-table tr:first-child td, #xlsx-table tr:first-child th { background: #e8f0fe; font-weight: 600; }
          </style><table`
        );
        setHtmlContent(styledHtml);
      } else if (docType === 'word') {
        if (!mammoth) {
          setError('Загрузка библиотеки для Word...');
          // wait a tick for the dynamic import
          await new Promise((r) => setTimeout(r, 500));
          if (!mammoth) { setError('Не удалось загрузить просмотрщик Word'); return; }
        }
        const buffer = await blob.arrayBuffer();
        const result = await mammoth.convertToHtml({ arrayBuffer: buffer });
        setHtmlContent(result.value || '<p>Документ пуст</p>');
      } else {
        const url = URL.createObjectURL(blob);
        setObjectUrl(url);
      }
    } catch (e) {
      console.error(e);
      setError('Ошибка при загрузке документа');
    } finally {
      setLoading(false);
    }
  }, [fileId, docType]);

  useEffect(() => {
    if (fileId) {
      setZoom(1);
      fetchAndRender();
    }
    return () => {
      setObjectUrl((prev) => { if (prev) URL.revokeObjectURL(prev); return null; });
    };
  }, [fileId]);

  const handleDownload = async () => {
    if (!fileId) return;
    const blob = await apiService.getBlob(`/v1/files/${fileId}`);
    if (blob) {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      a.click();
      URL.revokeObjectURL(url);
    }
  };

  const title = (
    <div className="flex items-center gap-3 pr-8">
      <span className="font-medium truncate max-w-xs">{fileName}</span>
      <Space>
        {docType === 'pdf' && (
          <>
            <Button size="small" icon={<ZoomOutOutlined />} onClick={() => setZoom((z) => Math.max(0.5, z - 0.25))} />
            <span className="text-sm">{Math.round(zoom * 100)}%</span>
            <Button size="small" icon={<ZoomInOutlined />} onClick={() => setZoom((z) => Math.min(3, z + 0.25))} />
          </>
        )}
        <Button size="small" icon={<DownloadOutlined />} onClick={handleDownload}>
          Скачать
        </Button>
      </Space>
    </div>
  );

  return (
    <Modal
      open={!!fileId}
      onCancel={onClose}
      title={title}
      footer={null}
      width="80vw"
      style={{ top: 20 }}
      styles={{ body: { padding: 0, height: '80vh', overflow: 'auto' } }}
    >
      {loading && (
        <div className="flex items-center justify-center h-64">
          <Spin size="large" tip="Загрузка документа..." />
        </div>
      )}

      {error && !loading && (
        <div className="p-4">
          <Alert type="error" message={error} />
        </div>
      )}

      {!loading && !error && docType === 'pdf' && objectUrl && (
        <div style={{ width: '100%', height: '100%', overflow: 'hidden' }}>
          <iframe
            src={objectUrl}
            title={fileName}
            style={{
              border: 'none',
              transform: `scale(${zoom})`,
              transformOrigin: 'top left',
              width: `${100 / zoom}%`,
              height: `${100 / zoom}%`,
            }}
          />
        </div>
      )}

      {!loading && !error && (docType === 'excel' || docType === 'word') && htmlContent && (
        <div
          className="p-4 prose max-w-none overflow-auto"
          style={{ height: '100%' }}
          dangerouslySetInnerHTML={{ __html: htmlContent }}
        />
      )}

      {!loading && !error && docType === 'other' && (
        <div className="flex flex-col items-center justify-center h-64 gap-4 text-gray-500">
          <p>Предпросмотр недоступен для этого типа файла.</p>
          <Button type="primary" icon={<DownloadOutlined />} onClick={handleDownload}>
            Скачать файл
          </Button>
        </div>
      )}
    </Modal>
  );
};

export default DocumentViewerModal;
