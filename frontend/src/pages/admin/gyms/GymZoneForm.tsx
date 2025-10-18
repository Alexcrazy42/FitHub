import React, { useState } from "react";
import { Button, Modal } from "antd";
import { useForm } from "react-hook-form";
import { IGymZoneResponse, IUpdateGymZoneRequest } from "../../../types/gyms";
import { useApiService } from "../../../api/useApiService";
import { toast } from "react-toastify";
import { ExclamationCircleOutlined } from "@ant-design/icons";

interface GymZoneFormProps {
  gymZone: IGymZoneResponse;
  onSave: (values: IUpdateGymZoneRequest) => Promise<IGymZoneResponse>;
  onCancel: () => void;
  loading: boolean;
  refreshAll : () => Promise<void>;
}

export const GymZoneForm: React.FC<GymZoneFormProps> = ({
  gymZone,
  onSave,
  onCancel,
  loading,
  refreshAll
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IUpdateGymZoneRequest>({
    defaultValues: {
      id: gymZone.id,
      name: gymZone.name,
      description: gymZone.description,
    },
  });
  const [deleteLoading, setDeleteLoading] = useState(false);
  const apiService = useApiService();
  const [modal, contextHolder] = Modal.useModal();

  const onSubmit = async (data: IUpdateGymZoneRequest) => {
    await onSave(data);
  };

  const deleteZone = async () => {
      setDeleteLoading(true);
      try {
        const response = await apiService.delete(`v1/gym-zones/${gymZone.id}`);
        if (response.success) {
          toast.success("Зал успешно удалён!");
          await refreshAll();
          
        } else {
          const error = response.error?.detail ?? "Ошибка при удалении зоны!";
          toast.error(error);
        }
      } catch (err) {
        toast.error("Ошибка при удалении зоны!");
      } finally {
        setDeleteLoading(false);
        onCancel();
      }
    };

  const confirmDelete = () => {
    modal.confirm({
      title: "Вы уверены, что хотите удалить этот зал?",
      icon: <ExclamationCircleOutlined />,
      content: `Это действие необратимо. Зона "${gymZone.name}" будет удалена без возможности восстановления.`,
      okText: "Удалить",
      okType: "danger",
      cancelText: "Отмена",
      onOk: async () => {
        await deleteZone();
      },
    });
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
          {...register("name", { required: "Введите название зоны" })}
          placeholder="Введите название"
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
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
          placeholder="Введите описание зоны"
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        {errors.description && (
          <p className="text-red-500 text-xs mt-1">
            {errors.description.message}
          </p>
        )}
      </div>

      <div className="flex space-x-2">
        <Button type="primary" htmlType="submit" loading={loading}>
          Сохранить
        </Button>
        <Button onClick={onCancel} disabled={loading}>
          Отмена
        </Button>
      </div>
    </form>
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
    </>
        
  );
};
