import React, { useState } from "react";
import { Button, Space, Upload, message } from "antd";
import { UploadOutlined } from "@ant-design/icons";
import { useForm } from "react-hook-form";
import { IGymResponse, IUpdateGymRequest } from "../../../types/gyms";
import { getFileRoute } from "../../../api/files";
import GymImageUploader from "./GymImageUploader";

interface GymFormProps {
  gym: IGymResponse;
  onSave: (values: IUpdateGymRequest) => Promise<IGymResponse>;
  onPhotoUpload: (gymId: string, file: File) => Promise<void>;
  onCancel: () => void;
  loading: boolean;
}

export const GymForm: React.FC<GymFormProps> = ({
  gym,
  onSave,
  onPhotoUpload,
  onCancel,
  loading,
}) => {
  const [photoLoading, setPhotoLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IUpdateGymRequest>({
    defaultValues: {
      id: gym.id,
      name: gym.name,
      description: gym.description,
    },
  });

  const onSubmit = async (data: IUpdateGymRequest) => {
    try {
      await onSave(data);
    } catch (error) {
      console.error('Ошибка в форме:', error);
    }
  };

  const handlePhotoUpload = async (file: File) => {
    try {
      setPhotoLoading(true);
      await onPhotoUpload(gym.id, file);
      message.success('Фотография успешно загружена');
    } catch (error) {
      console.error('Ошибка загрузки фото:', error);
      message.error('Ошибка при загрузке фотографии');
    } finally {
      setPhotoLoading(false);
    }
    return false; // Prevent default upload behavior
  };

  const customUploadRequest = async (options: any) => {
    const { file, onSuccess, onError } = options;
    try {
      await handlePhotoUpload(file);
      onSuccess("ok");
    } catch (error) {
      onError(error);
    }
  };

  const beforeUpload = (file: File) => {
    const isImage = file.type.startsWith('image/');
    if (!isImage) {
      message.error('Можно загружать только изображения!');
      return false;
    }

    const isLt5M = file.size / 1024 / 1024 < 5;
    if (!isLt5M) {
      message.error('Изображение должно быть меньше 5MB!');
      return false;
    }

    return true;
  };

  return (
    <>
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <input type="hidden" {...register("id")} />

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Название *
        </label>
        <input
          {...register("name", { required: "Введите название спортзала" })}
          placeholder="Введите название"
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
        {errors.name && (
          <p className="text-red-500 text-xs mt-1">{errors.name.message}</p>
        )}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Описание *
        </label>
        <textarea
          {...register("description", { required: "Введите описание" })}
          rows={4}
          placeholder="Введите описание спортзала"
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
        {errors.description && (
          <p className="text-red-500 text-xs mt-1">{errors.description.message}</p>
        )}
      </div>
      

      {/* Кнопки */}
      <div className="flex space-x-2">
        <Button 
          type="primary" 
          htmlType="submit" 
          loading={loading}
          disabled={loading || photoLoading}
        >
          Сохранить
        </Button>
        <Button 
          onClick={onCancel}
          disabled={loading || photoLoading}
        >
          Отмена
        </Button>
      </div>
    </form>

    <GymImageUploader gymId={gym.id} currentImageUrl={gym.imageUrl} />
    </>
  );
};