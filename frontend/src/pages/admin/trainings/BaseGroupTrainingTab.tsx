import { useEffect, useRef, useState } from "react";
import {
  IAttachPhoto,
  IBaseGroupTraining,
  ICreateBaseGroupTraining,
  ITrainingType,
} from "../../../types/trainings";
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import {
  Table,
  Pagination,
  Modal,
  Button,
  Input,
  InputNumber,
  Switch,
  Select,
  Space,
  Tag,
  Drawer,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useForm, Controller } from "react-hook-form";
import { mapServerValidationErrors } from "../../../api/ApiService";
import { toast } from "react-toastify";
import { ExclamationCircleOutlined } from "@ant-design/icons";
import ImageUploader, { ImageUploaderHandle } from "../../../components/ImageUploader/ImageUploader";
import { EntityType, IEntity } from "../../../types/files";
import { getFileRoute } from "../../../api/files";

interface BaseGroupTrainingTabProps {
  activeTab: string;
}

export const BaseGroupTrainingTab: React.FC<BaseGroupTrainingTabProps> = ({
  activeTab,
}) => {
  const [modal, contextHolder] = Modal.useModal();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [baseGroupTrainings, setBaseGroupTrainings] = useState<
    IBaseGroupTraining[]
  >([]);
  const [loading, setLoading] = useState(false);

  const [modalVisible, setModalVisible] = useState(false);
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [editingItem, setEditingItem] = useState<IBaseGroupTraining | null | undefined>(
    null
  );
  const [submitting, setSubmitting] = useState(false);
  const [trainingTypesOptions, setTrainingTypesOptions] = useState<ITrainingType[]>([]);
  const uploaderRef = useRef<ImageUploaderHandle>(null);
  const [maxFileCount, setMaxFileCount] = useState(1);
  const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);
  const [isWaitingToConfirmUpload, setIsWaitingToConfirmUpload] = useState<boolean>(false);

  const apiService = useApiService();

  const {
    control,
    handleSubmit,
    reset,
    setError,
    clearErrors,
  } = useForm<ICreateBaseGroupTraining>({
    defaultValues: {
      name: "",
      description: "",
      durationInMinutes: 30,
      complexity: 1,
      isActive: true,
      trainingTypeIds: [],
    },
  });

  const fetch = async () => {
    setLoading(true);
    try {
      const response = await apiService.get<ListResponse<IBaseGroupTraining>>(
        `/v1/base-group-trainings?PageNumber=${page}&PageSize=${pageSize}`
      );
      if (response.success) {
        setBaseGroupTrainings(response.data?.items ?? []);
        setTotal(response.data?.totalItems ?? 0);
      }
    } catch {
      toast.error("Неизвестная ошибка при загрузке списка");
    } finally {
      setLoading(false);
    }
  };

  const fetchOne = async (id : string) => {
    try {
      const response = await apiService.get<IBaseGroupTraining>(
        `/v1/base-group-trainings/${id}`
      );
      if (response.success) {
        setEditingItem(response.data);
      }
    } catch {
      toast.error("Неизвестная ошибка");
    }
  }

  const fetchMaxFileCount = async () => {
      const response = await apiService.get<IEntity>(`/v1/entities?entityType=${EntityType.BaseGroupTraining}`);
      if(response.success) {
        const maxCount = response.data?.maxFileCount;
        if(maxCount) {
          setMaxFileCount(maxCount);
        }
      }
  }

  const fileUploadedFromUploader = async (fileId: string): Promise<void> => {
    setUploadedFileId(fileId);
  };
  
  const fetchTrainingTypes = async () => {
    try {
      const resp = await apiService.get<ListResponse<ITrainingType>>(
        `/v1/training-types`
      );
      if (resp.success) {
        setTrainingTypesOptions(resp.data?.items ?? []);
      } else {
        console.warn("Не удалось получить training types: ", resp.error);
      }
    } catch (e) {
      console.warn(e);
    }
  };

  useEffect(() => {
    if (activeTab === "trainings") {
      fetch();
      fetchTrainingTypes();
      fetchMaxFileCount();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeTab, page, pageSize]);

  const deletePhoto = async () => {
      if(editingItem?.photoId) {
        const response = await apiService.delete(`/v1/base-group-trainings/photos?fileId=${editingItem.photoId}`);
        if(response.success) {
          await fetchOne(editingItem.id)
        } else {
          toast.error("Ошибка при удалении файла!")
        }
      } else {
        toast.error("Не содержит фотографии!");
      }
  }

  const handleAttachImage = async () => {
    if (!uploadedFileId) return;
    try {
      const request : IAttachPhoto = {
        fileId: uploadedFileId,
        baseGroupTrainingId: editingItem?.id
      }
      await apiService.post(`/v1/base-group-trainings/photos`, request);
      toast.success("Изображение успешно привязано!");
      setUploadedFileId(null);
      setIsWaitingToConfirmUpload(false);
      await fetchOne(editingItem?.id);
    } catch (error) {
      console.error(error);
      toast.error("Не получилось привязать изображение!")
    }
  };

  const handleRemoveImage = () => {
    setUploadedFileId(null);
    setIsWaitingToConfirmUpload(false);
  }

  const handleButtonClick = () => {
    uploaderRef.current?.openFileDialog();
  };

  const openCreateModal = () => {
    clearErrors();
    reset();
    setModalVisible(true);
  };

  const openEditDrawer = async (item: IBaseGroupTraining) => {
    await fetchOne(item.id);
    clearErrors();
    reset({
      name: item.name,
      description: item.description,
      durationInMinutes: item.durationInMinutes,
      complexity: item.complexity,
      isActive: item.isActive,
      trainingTypeIds: item.trainingTypes.map((t) => t.id),
    });
    setDrawerVisible(true);
  };

  const onCreateSubmit = async (payload: ICreateBaseGroupTraining) => {
    setSubmitting(true);
    try {
      const resp = await apiService.post<IBaseGroupTraining>(
        "/v1/base-group-trainings",
        payload
      );
      if (resp.success) {
        toast.success("Группа тренировок создана");
        setModalVisible(false);
        fetch();
      } else {
        const problem = resp.error;
        if (problem?.errors && problem.errors.length > 0) {
          mapServerValidationErrors(problem.errors, setError);
          toast.error(
            <div
              dangerouslySetInnerHTML={{
                __html: problem?.detail.replace(/\n/g, "<br />"),
              }}
            />
          );
        } else {
          toast.error(problem?.detail ?? "Ошибка создания");
        }
      }
    } catch {
      toast.error("Неизвестная ошибка при создании");
    } finally {
      setSubmitting(false);
    }
  };

  const onEditSubmit = async (payload: ICreateBaseGroupTraining) => {
    if (!editingItem) return;
    setSubmitting(true);
    try {
      const resp = await apiService.put<IBaseGroupTraining>(
        `/v1/base-group-trainings/${editingItem.id}`,
        payload
      );
      if (resp.success) {
        toast.success("Группа тренировок обновлена");
        setDrawerVisible(false);
        setEditingItem(null);
        fetch();
      } else {
        const problem = resp.error;
        if (problem?.errors && problem.errors.length > 0) {
          mapServerValidationErrors(problem.errors, setError);
          toast.error(
            <div
              dangerouslySetInnerHTML={{
                __html: problem?.detail.replace(/\n/g, "<br />"),
              }}
            />
          );
        } else {
          toast.error(problem?.detail ?? "Ошибка обновления");
        }
      }
    } catch {
      toast.error("Неизвестная ошибка при обновлении");
    } finally {
      setSubmitting(false);
    }
  };

  // 🔥 Удаление
  const onDeleteTraining = async () => {
    if (!editingItem) return;
    modal.confirm({
      title: "Удалить группу тренировок?",
      icon: <ExclamationCircleOutlined />,
      content: `Вы уверены, что хотите удалить "${editingItem.name}"?`,
      okText: "Удалить",
      cancelText: "Отмена",
      okButtonProps: { danger: true },
      async onOk() {
        try {
          const resp = await apiService.delete(
            `/v1/base-group-trainings/${editingItem.id}`
          );
          if (resp.success) {
            toast.success("Группа тренировок удалена");
            setDrawerVisible(false);
            setEditingItem(null);
            fetch();
          } else {
            toast.error(resp.error?.detail ?? "Ошибка удаления");
          }
        } catch {
          toast.error("Неизвестная ошибка при удалении");
        }
      },
    });
  };

  const columns: ColumnsType<IBaseGroupTraining> = [
    {
      title: "Название",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Описание",
      dataIndex: "description",
      key: "description",
      render: (text) => text ?? "-",
    },
    {
      title: "Продолжительность (мин)",
      dataIndex: "durationInMinutes",
      key: "durationInMinutes",
    },
    {
      title: "Сложность",
      dataIndex: "complexity",
      key: "complexity",
    },
    {
      title: "Активно",
      dataIndex: "isActive",
      key: "isActive",
      render: (v) =>
        v ? <Tag color="green">Активно</Tag> : <Tag color="error">Неактивно</Tag>,
    },
    {
      title: "Типы тренировок",
      dataIndex: "trainingTypes",
      key: "trainingTypes",
      render: (types: ITrainingType[]) => (
        <div className="flex flex-wrap gap-2">
          {types?.map((t) => (
            <Tag>
              {t.name}
            </Tag>
          ))}
        </div>
      ),
    },
  ];

  return (
    <div className="p-4">
      {contextHolder}
      <Space className="mb-4">
        <Button type="primary" onClick={openCreateModal}>
          Создать группу тренировок
        </Button>

        <Button type="primary" onClick={fetch}>
          Обновить
        </Button>
      </Space>

      <Table<IBaseGroupTraining>
        rowKey={(r) => r.id ?? ""}
        dataSource={baseGroupTrainings}
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

      {/* Modal создания */}
      <Modal
        title="Создать группу тренировок"
        open={modalVisible}
        onCancel={() => setModalVisible(false)}
        footer={null}
      >
        <form onSubmit={handleSubmit(onCreateSubmit)} className="space-y-3">
          <FormFields
            control={control}
            trainingTypesOptions={trainingTypesOptions}
            submitting={submitting}
          />
          <div className="flex justify-end gap-2">
            <Button onClick={() => setModalVisible(false)}>Отмена</Button>
            <Button type="primary" htmlType="submit" loading={submitting}>
              Создать
            </Button>
          </div>
        </form>
      </Modal>

      <Drawer
        title={
          editingItem ? `Редактировать: ${editingItem.name}` : "Редактирование"
        }
        width={480}
        onClose={() => setDrawerVisible(false)}
        open={drawerVisible}
      >
        <form onSubmit={handleSubmit(onEditSubmit)} className="space-y-3">
          <FormFields
            control={control}
            trainingTypesOptions={trainingTypesOptions}
            submitting={submitting}
          />

          <div className="flex justify-between mt-6">
            <Button danger onClick={onDeleteTraining}>
              Удалить
            </Button>
            <div className="flex gap-2">
              <Button onClick={() => setDrawerVisible(false)}>Отмена</Button>
              <Button type="primary" htmlType="submit" loading={submitting}>
                Сохранить
              </Button>
            </div>
          </div>
        </form>


        {editingItem?.photoId && (
          <div className="mb-4 mt-4">
            <p className="text-sm text-gray-600 mb-2">Текущее изображение:</p>
            <img
              src={getFileRoute(editingItem?.photoId)}
              alt="Фото"
              className="w-32 h-32 object-cover rounded shadow-sm"
            />
            <Button onClick={deletePhoto}>
              Удалить
            </Button>
          </div>
        )}
        
        {uploadedFileId && isWaitingToConfirmUpload && !editingItem?.photoId && (
          <div className="mb-4 mt-4 relative w-32 h-32">
            <button
              onClick={() => handleRemoveImage()}
              className="absolute top-0 right-0 w-6 h-6 bg-white text-gray-700 rounded-full flex items-center justify-center shadow hover:bg-gray-100 transition-colors"
            >
              ×
            </button>
      
            <img
              src={getFileRoute(uploadedFileId)}
              alt="Фото"
              className="w-32 h-32 object-cover rounded shadow-sm"
            />
      
            <Button onClick={handleAttachImage}>
              Привязать фото
            </Button>
          </div>
          )}
      
        {!uploadedFileId && !isWaitingToConfirmUpload && !editingItem?.photoId && (
          <Button type="primary" onClick={handleButtonClick}>
            Выбрать фото
          </Button>
        )}
      </Drawer>

      <ImageUploader ref={uploaderRef} maxFileCount={maxFileCount} fileUpload={fileUploadedFromUploader} onSuccessCancel={()=>{setIsWaitingToConfirmUpload(true)}} />
    </div>
  );
};

const FormFields: React.FC<{
  control: any;
  trainingTypesOptions: ITrainingType[];
  submitting: boolean;
}> = ({ control, trainingTypesOptions }) => {
  return (
    <>
      <div>
        <label className="block mb-1">Название</label>
        <Controller
          control={control}
          name="name"
          rules={{
            required: "Обязательно!",
            minLength: { value: 3, message: "Минимум 3 символа" },
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

      <div>
        <label className="block mb-1">Описание</label>
        <Controller
          control={control}
          name="description"
          render={({ field }) => <Input.TextArea {...field} rows={3} />}
        />
      </div>

      <div>
        <label className="block mb-1">Продолжительность (мин)</label>
        <Controller
          control={control}
          name="durationInMinutes"
          rules={{ min: { value: 1, message: "Должно быть >= 1" } }}
          render={({ field, fieldState }) => (
            <>
              <InputNumber {...field} min={1} style={{ width: "100%" }} />
              {fieldState.error && (
                <div className="text-red-600 mt-1 text-sm">
                  {fieldState.error.message}
                </div>
              )}
            </>
          )}
        />
      </div>

      <div>
        <label className="block mb-1">Сложность (1–3)</label>
        <Controller
          control={control}
          name="complexity"
          rules={{
            min: { value: 1, message: "Минимум 1" },
            max: { value: 3, message: "Максимум 3" },
          }}
          render={({ field, fieldState }) => (
            <>
              <InputNumber {...field} min={1} max={3} style={{ width: "100%" }} />
              {fieldState.error && (
                <div className="text-red-600 mt-1 text-sm">
                  {fieldState.error.message}
                </div>
              )}
            </>
          )}
        />
      </div>

      <div className="flex items-center gap-3">
        <label>Активно</label>
        <Controller
          control={control}
          name="isActive"
          render={({ field }) => (
            <Switch checked={field.value} onChange={field.onChange} />
          )}
        />
      </div>

      <div>
        <label className="block mb-1">Типы тренировок</label>
        <Controller
          control={control}
          name="trainingTypeIds"
          render={({ field, fieldState }) => (
            <>
              <Select
                mode="multiple"
                allowClear
                className="w-full"
                value={field.value}
                onChange={field.onChange}
                options={trainingTypesOptions.map((t) => ({
                  label: t.name,
                  value: t.id,
                }))}
              />
              {fieldState.error && (
                <div className="text-red-600 mt-1 text-sm">
                  {fieldState.error.message}
                </div>
              )}
            </>
          )}
        />
      </div>
    </>
  );
};
