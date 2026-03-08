import React, { useState } from 'react';
import { Button } from 'antd';
import {
  FilePdfOutlined,
  FileExcelOutlined,
  FileWordOutlined,
  FilePptOutlined,
  FileOutlined,
  EyeOutlined,
  DownloadOutlined,
} from '@ant-design/icons';
import { IMessageAttachmentResponse } from '../../../../../types/messaging';
import DocumentViewerModal from '../DocumentViewerModal';
import { useApiService } from '../../../../../api/useApiService';

interface DocumentData {
  fileId: {
    value: string;
  };
  fileName: string;
  fileSize: number;
  mimeType: string;
}

interface DocumentAttachmentPreviewProps {
  attachment: IMessageAttachmentResponse;
  isMyMessage?: boolean;
}

function getFileIcon(mimeType: string, fileName: string) {
  if (mimeType === 'application/pdf' || fileName.endsWith('.pdf'))
    return <FilePdfOutlined className="text-2xl text-red-500" />;
  if (
    mimeType.includes('spreadsheetml') ||
    mimeType.includes('ms-excel') ||
    fileName.endsWith('.xlsx') ||
    fileName.endsWith('.xls')
  )
    return <FileExcelOutlined className="text-2xl text-green-600" />;
  if (
    mimeType.includes('wordprocessingml') ||
    mimeType.includes('msword') ||
    fileName.endsWith('.docx') ||
    fileName.endsWith('.doc')
  )
    return <FileWordOutlined className="text-2xl text-blue-600" />;
  if (
    mimeType.includes('presentationml') ||
    mimeType.includes('powerpoint') ||
    fileName.endsWith('.pptx') ||
    fileName.endsWith('.ppt')
  )
    return <FilePptOutlined className="text-2xl text-orange-500" />;
  return <FileOutlined className="text-2xl text-gray-500" />;
}

function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function canPreview(mimeType: string, fileName: string): boolean {
  return (
    mimeType === 'application/pdf' ||
    fileName.endsWith('.pdf') ||
    mimeType.includes('spreadsheetml') ||
    mimeType.includes('ms-excel') ||
    fileName.endsWith('.xlsx') ||
    fileName.endsWith('.xls') ||
    mimeType.includes('wordprocessingml') ||
    mimeType.includes('msword') ||
    fileName.endsWith('.docx') ||
    fileName.endsWith('.doc')
  );
}

export const DocumentAttachmentPreview: React.FC<DocumentAttachmentPreviewProps> = ({
  attachment,
  isMyMessage,
}) => {
  const apiService = useApiService();
  const [viewerOpen, setViewerOpen] = useState(false);

  const data: DocumentData = JSON.parse(attachment.data);

  const handleDownload = async () => {
    const blob = await apiService.getBlob(`/v1/files/${data.fileId.value}`);
    if (blob) {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = data.fileName;
      a.click();
      URL.revokeObjectURL(url);
    }
  };

  const bg = isMyMessage ? 'bg-blue-600/10 border-blue-300' : 'bg-gray-100 border-gray-200';

  return (
    <>
      <div
        className={`flex items-center gap-3 px-3 py-2 rounded-xl border ${bg} max-w-xs`}
      >
        <div className="flex-shrink-0">{getFileIcon(data.mimeType, data.fileName)}</div>
        <div className="flex-1 min-w-0">
          <p className="text-sm font-medium truncate">{data.fileName}</p>
          <p className="text-xs text-gray-500">{formatBytes(data.fileSize)}</p>
        </div>
        <div className="flex flex-col gap-1">
          {canPreview(data.mimeType, data.fileName) && (
            <Button
              size="small"
              type="text"
              icon={<EyeOutlined />}
              onClick={() => setViewerOpen(true)}
              title="Открыть"
            />
          )}
          <Button
            size="small"
            type="text"
            icon={<DownloadOutlined />}
            onClick={handleDownload}
            title="Скачать"
          />
        </div>
      </div>

      <DocumentViewerModal
        fileId={viewerOpen ? data.fileId.value : null}
        fileName={data.fileName}
        mimeType={data.mimeType}
        onClose={() => setViewerOpen(false)}
      />
    </>
  );
};
