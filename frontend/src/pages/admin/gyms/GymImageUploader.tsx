import React, { useState } from "react";
import { Upload, Button, Modal, message } from "antd";
import { UploadOutlined, LinkOutlined } from "@ant-design/icons";
import axios from "axios";
import { useApiService } from "../../../api/useApiService";
import { EntityType, IMakeFilesActiveRequest, IPresignedUrlResponse } from "../../../types/files";
import { getFileRoute } from "../../../api/files";

interface GymImageUploaderProps {
  gymId: string;
  currentImageUrl?: string;
}

const GymImageUploader: React.FC<GymImageUploaderProps> = ({
  gymId,
  currentImageUrl
}) => {
  const [photoLoading, setPhotoLoading] = useState(false);
  const [previewVisible, setPreviewVisible] = useState(false);
  const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);
  const [attaching, setAttaching] = useState(false);
  const apiService = useApiService();

  const customUploadRequest = async ({ file }: any) => {
    try {
        setPhotoLoading(true);

        // 1️⃣ готовим FormData для API
        const formData = new FormData();
        formData.append("File", file);

        const response = await apiService.postFormData<IPresignedUrlResponse>("/v1/files/get-presigned-url", formData);

        if(response.success) {
          const data = response.data;
          if(data !== null && data !== undefined) {
              await axios.put(data?.url, file, {
              headers: { "Content-Type": file.type },
            });
            await apiService.post(`/v1/files/${data?.fileId}/confirm-upload`, [data?.fileId]);

            setUploadedFileId(data.fileId);
            setPreviewVisible(true);
            message.success("Файл успешно загружен");
          }
        }
        
    } catch (error) {
        console.error(error);
        message.error("Ошибка при загрузке файла");
    } finally {
        setPhotoLoading(false);
    }
    };

  const handleAttachImage = async () => {
    if (!uploadedFileId) return;
    try {
      setAttaching(true);
      const makeFilesActiveRequest : IMakeFilesActiveRequest = {
        fileIds : [uploadedFileId],
        entityId: gymId,
        entityType: EntityType.Gym
      }
      await apiService.post(`/v1/files/make-files-active`, makeFilesActiveRequest);
      message.success("Изображение привязано к залу");
      console.log('Изображение привязано к залу')
      setPreviewVisible(false);
    } catch (error) {
      console.error(error);
      message.error("Не удалось привязать изображение");
    } finally {
      setAttaching(false);
    }
  };

  return (
    <div className="mt-2">
      <Upload
        name="image"
        customRequest={customUploadRequest}
        showUploadList={false}
        accept="image/*"
        disabled={photoLoading}
      >
        <Button
          icon={<UploadOutlined />}
          loading={photoLoading}
          disabled={photoLoading}
        >
          {photoLoading ? "Загрузка..." : "Загрузить изображение"}
        </Button>
      </Upload>

      <p className="text-xs text-gray-500 mt-1">
        Максимальный размер: 5MB. Форматы: JPG, PNG, WebP
      </p>

      {currentImageUrl && (
        <div className="mb-4 mt-4">
          <p className="text-sm text-gray-600 mb-2">Текущее изображение:</p>
          <img
            src={currentImageUrl}
            alt="Current gym"
            className="w-32 h-32 object-cover rounded shadow-sm"
          />
        </div>
      )}

      <Modal
        open={previewVisible}
        footer={[
          <Button
            key="confirm"
            type="primary"
            icon={<LinkOutlined />}
            onClick={handleAttachImage}
            loading={attaching}
          >
            Привязать изображение
          </Button>,
        ]}
        onCancel={() => setPreviewVisible(false)}
        title="Подтвердите изображение"
      >
        {uploadedFileId && (
          <img
            src={getFileRoute(uploadedFileId)}
            alt="preview"
            className="w-full rounded-lg"
          />
        )}
      </Modal>
    </div>
  );
};

export default GymImageUploader;
