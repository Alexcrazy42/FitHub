import React, { useEffect, useState } from "react";
import {
  Table,
  Pagination,
  Spin,
  Button,
  Modal,
  Form,
  Input,
  Space,
  Tag,
  Switch,
  DatePicker,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import {
  IEquipmentResponse,
  ICreateEquipmentRequest,
} from "../../../types/equipments";
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import { useForm, Controller } from "react-hook-form";
import {
  ExclamationCircleOutlined,
  EditOutlined,
  DeleteOutlined,
  PlusOutlined,
} from "@ant-design/icons";
import { toast } from "react-toastify";
import { BrandSelect } from "./components/BrandSelect";

interface EquipmentTabProps {
  activeTab: string;
}

export const EquipmentTab: React.FC<EquipmentTabProps> = ({ activeTab }) => {
  const [equipments, setEquipments] = useState<IEquipmentResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [modal, contextHolder] = Modal.useModal();

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  const [editingEquipment, setEditingEquipment] =
    useState<IEquipmentResponse | null>(null);

  const apiService = useApiService();
  const { control, handleSubmit, reset } =
    useForm<ICreateEquipmentRequest>();

  const fetchEquipments = async (page: number, pageSize: number) => {
    const response = await apiService.get<ListResponse<IEquipmentResponse>>(
      `/v1/equipments?PageNumber=${page}&PageSize=${pageSize}`
    );
    if (response.success) {
      const items = response.data?.items || [];
      const total = response.data?.totalItems || 0;
      return { data: items, total };
    }
    return { data: [], total: 0 };
  };

  const loadEquipments = async () => {
    setLoading(true);
    fetchEquipments(page, pageSize)
      .then((res) => {
        setEquipments(res.data);
        setTotal(res.total);
      })
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    if (activeTab === "equipments") {
      loadEquipments();
    }
  }, [activeTab, page, pageSize]);

  const onSubmit = async (data: ICreateEquipmentRequest) => {
    try {
      const payload: ICreateEquipmentRequest = {
        ...data,
        instructionAddBefore: data.instructionAddBefore
          ? dayjs(data.instructionAddBefore).format("YYYY-MM-DD")
          : null,
      };

      if (isEditMode && editingEquipment) {
        const response = await apiService.put<IEquipmentResponse>(
          `/v1/equipments/${editingEquipment.id}`,
          payload
        );
        if (response.success) {
          toast.success("Оборудование обновлено!");
          setIsModalOpen(false);
          setEditingEquipment(null);
          reset();
          loadEquipments();
        } else {
          toast.error(response.error?.detail || "Ошибка при обновлении");
        }
      } else {
        const response = await apiService.post<IEquipmentResponse>(
          "/v1/equipments",
          payload
        );
        if (response.success) {
          toast.success("Оборудование создано!");
          setIsModalOpen(false);
          reset();
          loadEquipments();
        } else {
          toast.error(response.error?.detail || "Ошибка при создании");
        }
      }
    } catch (e) {
      console.error(e);
      toast.error("Ошибка при сохранении оборудования");
    }
  };

  const handleDeleteEquipment = async (equipment: IEquipmentResponse) => {
    modal.confirm({
      title: `Удалить оборудование "${equipment.name}"?`,
      icon: <ExclamationCircleOutlined />,
      content: "Это действие нельзя будет отменить.",
      okText: "Удалить",
      okType: "danger",
      cancelText: "Отмена",
      async onOk() {
        try {
          const response = await apiService.delete(
            `/v1/equipments/${equipment.id}`
          );
          if (response.success) {
            toast.success("Оборудование удалено");
            loadEquipments();
          } else {
            toast.error(response.error?.detail || "Ошибка при удалении");
          }
        } catch (e) {
          console.error(e);
          toast.error("Ошибка при удалении оборудования");
        }
      },
    });
  };

  const openEditModal = (equipment: IEquipmentResponse) => {
  setIsEditMode(true);
  setEditingEquipment(equipment);
  setIsModalOpen(true);

  reset({
    brandId: equipment.brand.id,
    name: equipment.name,
    description: equipment.description,
    additionalDescroption: equipment.additionalDescroption || "",
    instructionAddBefore: equipment.instructionAddBefore || null,
    isActive: equipment.isActive,
  });
};

  const columns: ColumnsType<IEquipmentResponse> = [
    {
      title: "Название",
      dataIndex: "name",
      key: "name",
      width: "20%",
    },
    {
      title: "Описание",
      dataIndex: "description",
      key: "description",
      width: "25%",
    },
    {
      title: "Бренд",
      dataIndex: ["brand", "name"],
      key: "brand",
      width: "15%",
    },
    {
      title: "Инструкция до",
      dataIndex: "instructionAddBefore",
      key: "instructionAddBefore",
      render: (text) => (text ? dayjs(text).format("DD.MM.YYYY") : "—"),
      width: "15%",
    },
    {
      title: "Статус",
      dataIndex: "isActive",
      key: "isActive",
      width: "10%",
      align: "center",
      render: (value: boolean) =>
        value ? (
          <Tag color="green">Активно</Tag>
        ) : (
          <Tag color="default">Неактивно</Tag>
        ),
    },
    {
      title: "Действия",
      key: "actions",
      width: "15%",
      align: "center",
      render: (_, record) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => openEditModal(record)}
          >
            Редактировать
          </Button>
          {contextHolder}
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() => handleDeleteEquipment(record)}
          >
            Удалить
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-lg font-semibold">Тренажеры</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => {
            setIsModalOpen(true);
            setIsEditMode(false);
            setEditingEquipment(null);
            reset();
          }}
        >
          Добавить тренажер
        </Button>
      </div>

      {loading ? (
        <div className="flex justify-center items-center py-10">
          <Spin />
        </div>
      ) : (
        <>
          <Table
            dataSource={equipments}
            columns={columns}
            pagination={false}
            rowKey="id"
            bordered
          />
          <div className="flex justify-end mt-4">
            <Pagination
              current={page}
              pageSize={pageSize}
              total={total}
              showSizeChanger
              onChange={(p, size) => {
                setPage(p);
                setPageSize(size || 10);
              }}
            />
          </div>
        </>
      )}

      {/* Modal для создания / редактирования */}
      <Modal
        title={isEditMode ? "Редактировать оборудование" : "Добавить оборудование"}
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          setEditingEquipment(null);
          reset();
        }}
        footer={null}
      >
        <Form layout="vertical" onFinish={handleSubmit(onSubmit)}>
          <Form.Item label="Бренд" required>
            <BrandSelect control={control} name="brandId" apiService={apiService} />
          </Form.Item>

          <Form.Item label="Название" required>
            <Controller
              name="name"
              control={control}
              rules={{ required: "Введите название" }}
              render={({ field, fieldState }) => (
                <>
                  <Input
                    {...field}
                    placeholder="Название оборудования"
                    value={field.value ?? ""} // заменяем null на пустую строку
                  />
                  {fieldState.error && (
                    <p className="text-red-500 text-sm mt-1">
                      {fieldState.error.message}
                    </p>
                  )}
                </>
              )}
            />
          </Form.Item>

          <Form.Item label="Описание">
            <Controller
              name="description"
              control={control}
              render={({ field }) => (
                <Input.TextArea {...field} value={field.value ?? ""} placeholder="Описание" rows={3} />
              )}
            />
          </Form.Item>

          <Form.Item label="Доп. описание">
            <Controller
              name="additionalDescroption"
              control={control}
              render={({ field }) => (
                <Input.TextArea {...field} value={field.value ?? ""} placeholder="Доп. описание" rows={2} />
              )}
            />
          </Form.Item>

          <Form.Item label="Инструкция до">
            <Controller
              name="instructionAddBefore"
              control={control}
              render={({ field }) => (
                <DatePicker
                  {...field}
                  format="YYYY-MM-DD"
                  value={field.value ? dayjs(field.value) : null}
                  onChange={(date) =>
                    field.onChange(date ? date.format("YYYY-MM-DD") : null)
                  }
                />
              )}
            />
          </Form.Item>

          <Form.Item label="Активно">
            <Controller
              name="isActive"
              control={control}
              render={({ field }) => (
                <Switch
                  checked={!!field.value}
                  onChange={(checked) => field.onChange(checked)}
                />
              )}
            />
          </Form.Item>

          <div className="flex justify-end gap-2">
            <Button onClick={() => setIsModalOpen(false)}>Отмена</Button>
            <Button type="primary" htmlType="submit">
              {isEditMode ? "Сохранить" : "Создать"}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
