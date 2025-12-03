import { useEffect, useState } from "react";
import {
  ITrainingType,
  ICreateTrainingType,
} from "../../../types/trainings";
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import {
  Table,
  Pagination,
  Modal,
  Button,
  Input,
  Space,
  Drawer,
  Popconfirm,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useForm, Controller } from "react-hook-form";
import { mapServerValidationErrors } from "../../../api/ApiService";
import { toast } from "react-toastify";

interface TrainingTypeTabProps {
  activeTab: string;
}

export const TrainingTypeTab: React.FC<TrainingTypeTabProps> = ({
  activeTab,
}) => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [trainingTypes, setTrainingTypes] = useState<ITrainingType[]>([]);
  const [loading, setLoading] = useState(false);

  const [modalVisible, setModalVisible] = useState(false);
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [editingItem, setEditingItem] = useState<ITrainingType | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const apiService = useApiService();

  const {
    control,
    handleSubmit,
    reset,
    setError,
    clearErrors,
  } = useForm<ICreateTrainingType>({
    defaultValues: { name: "" },
  });

  const fetch = async () => {
    setLoading(true);
    try {
      const response = await apiService.get<ListResponse<ITrainingType>>(
        `/v1/training-types`
      );
      if (response.success) {
        setTrainingTypes(response.data?.items ?? []);
        setTotal(response.data?.totalItems ?? response.data?.items.length ?? 10);
      }
    } catch {
      toast.error("Ошибка при загрузке типов тренировок");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (activeTab === "trainingTypes") {
      fetch();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeTab, page, pageSize]);

  const openCreateModal = () => {
    clearErrors();
    reset({ name: "" });
    setModalVisible(true);
  };

  const openEditDrawer = (item: ITrainingType) => {
    setEditingItem(item);
    clearErrors();
    reset({ name: item.name ?? "" });
    setDrawerVisible(true);
  };

  const onCreateSubmit = async (payload: ICreateTrainingType) => {
    setSubmitting(true);
    try {
      const resp = await apiService.post<ITrainingType>(
        "/v1/training-types",
        payload
      );
      if (resp.success) {
        toast.success("Тип тренировки создан");
        setModalVisible(false);
        fetch();
      } else {
        const problem = resp.error;
        if (problem?.errors?.length) {
          mapServerValidationErrors(problem.errors, setError);
        }
        toast.error(problem?.detail ?? "Ошибка создания");
      }
    } catch {
      toast.error("Неизвестная ошибка при создании");
    } finally {
      setSubmitting(false);
    }
  };

  const onEditSubmit = async (payload: ICreateTrainingType) => {
    if (!editingItem) return;
    setSubmitting(true);
    try {
      const resp = await apiService.put<ITrainingType>(
        `/v1/training-types/${editingItem.id}`,
        payload
      );
      if (resp.success) {
        toast.success("Тип тренировки обновлен");
        setDrawerVisible(false);
        setEditingItem(null);
        fetch();
      } else {
        const problem = resp.error;
        if (problem?.errors?.length) {
          mapServerValidationErrors(problem.errors, setError);
        }
        toast.error(problem?.detail ?? "Ошибка обновления");
      }
    } catch {
      toast.error("Неизвестная ошибка при обновлении");
    } finally {
      setSubmitting(false);
    }
  };

  const onDelete = async () => {
    if (!editingItem) return;
    setSubmitting(true);
    try {
      const resp = await apiService.delete(`/v1/training-types/${editingItem.id}`);
      if (resp.success) {
        toast.success("Тип тренировки удалён");
        setDrawerVisible(false);
        setEditingItem(null);
        fetch();
      } else {
        toast.error(resp.error?.detail ?? "Ошибка удаления");
      }
    } catch {
      toast.error("Неизвестная ошибка при удалении");
    } finally {
      setSubmitting(false);
    }
  };

  const columns: ColumnsType<ITrainingType> = [
    {
      title: "Название типа тренировки",
      dataIndex: "name",
      key: "name",
      render: (text) => text ?? "-",
    },
  ];

  return (
    <div className="p-4">
      <Space className="mb-4">
        <Button type="primary" onClick={openCreateModal}>
          Создать тип тренировки
        </Button>
      </Space>

      <Table<ITrainingType>
        rowKey={(r) => r.id ?? ""}
        dataSource={trainingTypes}
        columns={columns}
        loading={loading}
        pagination={false}
        onRow={(record) => ({
          onClick: () => openEditDrawer(record),
          style: { cursor: "pointer" },
        })}
      />

      <div className="flex justify-end mt-4">
        <Pagination
          current={page}
          pageSize={pageSize}
          total={total}
          onChange={(p, ps) => {
            setPage(p);
            setPageSize(ps);
          }}
        />
      </div>

      {/* Модалка создания */}
      <Modal
        title="Создать тип тренировки"
        open={modalVisible}
        onCancel={() => setModalVisible(false)}
        footer={null}
      >
        <form onSubmit={handleSubmit(onCreateSubmit)} className="space-y-3">
          <FormFields control={control} />
          <div className="flex justify-end gap-2">
            <Button onClick={() => setModalVisible(false)}>Отмена</Button>
            <Button type="primary" htmlType="submit" loading={submitting}>
              Создать
            </Button>
          </div>
        </form>
      </Modal>

      {/* Drawer редактирования */}
      <Drawer
        title={
          editingItem ? `Редактировать: ${editingItem.name}` : "Редактирование"
        }
        width={400}
        onClose={() => setDrawerVisible(false)}
        open={drawerVisible}
      >
        <form onSubmit={handleSubmit(onEditSubmit)} className="space-y-3">
          <FormFields control={control} />
          <div className="flex justify-between mt-6">
            <Popconfirm
              title="Удалить тип тренировки?"
              okText="Да"
              cancelText="Нет"
              onConfirm={onDelete}
            >
              <Button danger loading={submitting}>
                Удалить
              </Button>
            </Popconfirm>

            <div className="flex gap-2">
              <Button onClick={() => setDrawerVisible(false)}>Отмена</Button>
              <Button type="primary" htmlType="submit" loading={submitting}>
                Сохранить
              </Button>
            </div>
          </div>
        </form>
      </Drawer>
    </div>
  );
};

const FormFields: React.FC<{ control: any }> = ({ control }) => (
  <div>
    <label className="block mb-1">Название</label>
    <Controller
      control={control}
      name="name"
      rules={{
        required: "Обязательно!",
        minLength: { value: 2, message: "Минимум 2 символа" },
      }}
      render={({ field, fieldState }) => (
        <>
          <Input {...field} />
          {fieldState.error && (
            <div className="text-red-600 mt-1 text-sm">
              {fieldState.error.message}
            </div>
          )}
        </>
      )}
    />
  </div>
);
