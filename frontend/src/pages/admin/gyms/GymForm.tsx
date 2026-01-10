import React, { useRef, useState, useEffect } from "react";
import { Button, Modal } from "antd";
import { useForm } from "react-hook-form";
import { IGymResponse, IUpdateGymRequest } from "../../../types/gyms";
import { getFileRoute } from "../../../api/files";
import ImageUploader, { ImageUploaderHandle } from "../../../components/ImageUploader/ImageUploader";
import { EntityType, IEntity, IEntityType, IMakeFilesActiveRequest } from "../../../types/files";
import { useApiService } from "../../../api/useApiService";
import { toast } from "react-toastify";
import { ExclamationCircleOutlined } from "@ant-design/icons";

interface GymFormProps {
  gym: IGymResponse;
  onSave: (values: IUpdateGymRequest) => Promise<IGymResponse>;
  onCancel: () => void;
  loading: boolean;
  refresh : (gym: IGymResponse) => Promise<IGymResponse>;
  refreshAll : () => Promise<void>;
}

export const GymForm: React.FC<GymFormProps> = ({
  gym,
  onSave,
  onCancel,
  loading,
  refresh,
  refreshAll
}) => {
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
  const uploaderRef = useRef<ImageUploaderHandle>(null);
  const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);
  const [isWaitingToConfirmUpload, setIsWaitingToConfirmUpload] = useState<boolean>(false);
  const apiService = useApiService();
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [modal, contextHolder] = Modal.useModal();
  const [maxFileCount, setMaxFileCount] = useState(1);

  const onSubmit = async (data: IUpdateGymRequest) => {
    try {
      await onSave(data);
    } catch (error) {
      console.error('Ошибка в форме:', error);
    }
  };

  const handleButtonClick = () => {
    uploaderRef.current?.openFileDialog();
  };

  const fileUploadedFromUploader = async (fileId: string): Promise<void> => {
    setUploadedFileId(fileId);
  };

  const handleRemoveImage = () => {
    setUploadedFileId(null);
    setIsWaitingToConfirmUpload(false);
  }

  const handleAttachImage = async () => {
      if (!uploadedFileId) return;
      try {
        const makeFilesActiveRequest : IMakeFilesActiveRequest = {
          fileIds : [uploadedFileId],
          entityId: gym.id,
          entityType: EntityType.Gym
        }
        await apiService.post(`/v1/files/make-files-active`, makeFilesActiveRequest);
        toast.success("Изображение успешно привязано к залу!");
        setUploadedFileId(null);
        setIsWaitingToConfirmUpload(false);
        await refresh(gym);
      } catch (error) {
        console.error(error);
        toast.error("Не получилось привязать изображение!")
      }
  };

  const deleteGymPhoto = async () => {
    if(gym.imageFileId) {
      const response = await apiService.delete(`v1/files/${gym.imageFileId}`);
      if(response.success) {
        await refresh(gym);
      } else {
        toast.error("Ошибка при удалении файла!")
      }
    } else {
      toast.error("Зал не содержит картинки!");
    }
  }

  const deleteGym = async () => {
    setDeleteLoading(true);
    try {
      const response = await apiService.delete(`v1/gyms/${gym.id}`);
      if (response.success) {
        toast.success("Зал успешно удалён!");
        await refreshAll();
        
      } else {
        const error = response.error?.detail ?? "Ошибка при удалении зала!";
        toast.error(error);
      }
    } catch (err) {
      toast.error("Ошибка при удалении зала!");
    } finally {
      setDeleteLoading(false);
      onCancel();
    }
  };

   const confirmDelete = () => {
    modal.confirm({
      title: "Вы уверены, что хотите удалить этот зал?",
      icon: <ExclamationCircleOutlined />,
      content: `Это действие необратимо. Зал "${gym.name}" будет удалён без возможности восстановления.`,
      okText: "Удалить",
      okType: "danger",
      cancelText: "Отмена",
      onOk: async () => {
        await deleteGym(); // ✅ ждем завершения
      },
    });
  };

  const fetchMaxFileCount = async () => {
    const response = await apiService.get<IEntity>(`/v1/entities?entityType=${EntityType.Gym}`);
    if(response.success) {
      const maxCount = response.data?.maxFileCount;
      if(maxCount) {
        setMaxFileCount(maxCount);
      }
    }
  }

  useEffect(() => {
    fetchMaxFileCount();
  }, []);

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
        <label className="block text-sm font-medium text-gray-700 mb-1">а
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
      

      <div className="flex space-x-2">
        <Button 
          type="primary"
          htmlType="submit" 
          loading={loading}
          disabled={loading}
        >
          Сохранить
        </Button>
        <Button 
          onClick={onCancel}
          disabled={loading}
        >
          Отмена
        </Button>
      </div>
    </form>


    {gym.imageFileId && (
        <div className="mb-4 mt-4">
          <p className="text-sm text-gray-600 mb-2">Текущее изображение:</p>
          <img
            src={getFileRoute(gym.imageFileId)}
            alt="Current gym"
            className="w-32 h-32 object-cover rounded shadow-sm"
          />
          <Button onClick={deleteGymPhoto}>
            Удалить
          </Button>
        </div>
      )}

  {uploadedFileId && isWaitingToConfirmUpload && !gym.imageFileId && (
    <div className="mb-4 mt-4 relative w-32 h-32">
      <button
        onClick={() => handleRemoveImage()}
        className="absolute top-0 right-0 w-6 h-6 bg-white text-gray-700 rounded-full flex items-center justify-center shadow hover:bg-gray-100 transition-colors"
      >
        ×
      </button>

      <img
        src={getFileRoute(uploadedFileId)}
        alt="Current gym"
        className="w-32 h-32 object-cover rounded shadow-sm"
      />

      <Button onClick={handleAttachImage}>
        Привязать фото к залу
      </Button>
    </div>
    )}

    {!uploadedFileId && !isWaitingToConfirmUpload && !gym.imageFileId && (
      <Button type="primary" onClick={handleButtonClick}>
        Выбрать фото
      </Button>
    )}
  
  {contextHolder}
  <Button
      type="primary"
      danger
      loading={deleteLoading}
      disabled={deleteLoading}
      onClick={confirmDelete}
    >
      Удалить
    </Button>
    
    <ImageUploader ref={uploaderRef} maxFileCount={maxFileCount} fileUpload={fileUploadedFromUploader} onSuccessCancel={()=>{setIsWaitingToConfirmUpload(true)}} />
    </>
  );
};