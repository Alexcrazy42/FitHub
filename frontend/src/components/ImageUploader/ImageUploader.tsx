import React, { useRef, useImperativeHandle, forwardRef } from "react";
import { useApiService } from "../../api/useApiService";
import { IPresignedUrlResponse } from "../../types/files";
import axios from "axios";
import { message } from "antd";
import { toast } from "react-toastify";

interface ImageUploaderProps {
  maxFileCount: number;
  fileUpload: (fileId: string) => Promise<void>;
  onSuccessCancel: () => void;
}

export interface ImageUploaderHandle {
  openFileDialog: () => void;
}

const ImageUploader = forwardRef<ImageUploaderHandle, ImageUploaderProps>(
  ({ maxFileCount, fileUpload, onSuccessCancel: onCancel }, ref) => {
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const apiService = useApiService();

    useImperativeHandle(ref, () => ({
      openFileDialog: () => {
        fileInputRef.current?.click();
      },
    }));

    const customUploadRequest = async (files: File[]) => {
      if (!files || files.length === 0) return;

      try {
        const uploadPromises = files.map(async (file) => {
          const formData = new FormData();
          formData.append("File", file);

          const response = await apiService.postFormData<IPresignedUrlResponse>(
            "/v1/files/get-presigned-url",
            formData
          );

          if (response.success && response.data) {
            const { url, fileId } = response.data;

            await axios.put(url, file, {
              headers: { "Content-Type": file.type },
            });

            await apiService.post(`/v1/files/${fileId}/confirm-upload`, [fileId]);

            message.success(`Файл "${file.name}" успешно загружен`);
            await fileUpload(fileId);
          }
        });

        await Promise.all(uploadPromises);
        onCancel();
      } catch (error: unknown) {
        toast.error("Ошибка при загрузке файлов");
        console.error(error);
      }
    };

    const handleSelectFile = async (e: React.ChangeEvent<HTMLInputElement>) => {
      const files = e.target.files;
      if (!files) return;

      if (files.length > maxFileCount) {
        toast.error(`Можно загрузить максимум ${maxFileCount} файлов!`);
        e.target.value = "";
        return;
      }

      await customUploadRequest(Array.from(files));
      e.target.value = "";
    };

    return <input 
      ref={fileInputRef} 
      type="file" 
      accept="image/*" 
      hidden
      multiple={maxFileCount > 1} onChange={handleSelectFile} />;
  }
);

export default ImageUploader;