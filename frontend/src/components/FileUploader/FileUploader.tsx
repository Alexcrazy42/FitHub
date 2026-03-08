import React, { useRef, useImperativeHandle, forwardRef } from 'react';
import { useApiService } from '../../api/useApiService';
import { IPresignedUrlResponse } from '../../types/files';
import axios from 'axios';
import { message } from 'antd';
import { toast } from 'react-toastify';

export interface FileUploadResult {
  fileId: string;
  fileName: string;
  fileSize: number;
  mimeType: string;
}

interface FileUploaderProps {
  accept: string;
  maxFileCount: number;
  onFileUploaded: (result: FileUploadResult) => Promise<void>;
  onDone?: () => void;
}

export interface FileUploaderHandle {
  openFileDialog: () => void;
}

const FileUploader = forwardRef<FileUploaderHandle, FileUploaderProps>(
  ({ accept, maxFileCount, onFileUploaded, onDone }, ref) => {
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const apiService = useApiService();

    useImperativeHandle(ref, () => ({
      openFileDialog: () => {
        fileInputRef.current?.click();
      },
    }));

    const uploadFiles = async (files: File[]) => {
      if (!files || files.length === 0) return;

      try {
        const uploads = files.map(async (file) => {
          const formData = new FormData();
          formData.append('File', file);

          const response = await apiService.postFormData<IPresignedUrlResponse>(
            '/v1/files/get-presigned-url',
            formData
          );

          if (response.success && response.data) {
            const { url, fileId } = response.data;

            await axios.put(url, file, {
              headers: { 'Content-Type': file.type },
            });

            await apiService.post(`/v1/files/${fileId}/confirm-upload`, [fileId]);

            message.success(`"${file.name}" загружен`);
            await onFileUploaded({
              fileId,
              fileName: file.name,
              fileSize: file.size,
              mimeType: file.type || 'application/octet-stream',
            });
          }
        });

        await Promise.all(uploads);
        onDone?.();
      } catch (error: unknown) {
        toast.error('Ошибка при загрузке файла');
        console.error(error);
      }
    };

    const handleChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
      const files = e.target.files;
      if (!files) return;

      if (files.length > maxFileCount) {
        toast.error(`Максимум ${maxFileCount} файлов`);
        e.target.value = '';
        return;
      }

      await uploadFiles(Array.from(files));
      e.target.value = '';
    };

    return (
      <input
        ref={fileInputRef}
        type="file"
        accept={accept}
        hidden
        multiple={maxFileCount > 1}
        onChange={handleChange}
      />
    );
  }
);

export default FileUploader;
