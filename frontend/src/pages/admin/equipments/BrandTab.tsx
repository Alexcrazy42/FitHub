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
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { IBrandResponse } from "../../../types/equipments";
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import { useForm, Controller } from "react-hook-form";
import { toast } from "react-toastify";
import { ExclamationCircleOutlined, EditOutlined, DeleteOutlined, PlusOutlined } from "@ant-design/icons";

interface BrandTabProps {
  activeTab: string;
}

export const BrandTab: React.FC<BrandTabProps> = ({ activeTab }) => {
  const [brands, setBrands] = useState<IBrandResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [modal, contextHolder] = Modal.useModal();

  // Модалка
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  const [editingBrand, setEditingBrand] = useState<IBrandResponse | null>(null);

  const apiService = useApiService();
  const { control, handleSubmit, reset, setValue } = useForm<IBrandResponse>();

  const fetchBrands = async (page: number, pageSize: number) => {
    const response = await apiService.get<ListResponse<IBrandResponse>>(
      `/v1/brands?PageNumber=${page}&PageSize=${pageSize}`
    );
    if (response.success) {
      const brands = response.data?.items || [];
      const total = response.data?.totalItems || 0;
      return { data: brands, total };
    }
    return { data: [], total: 0 };
  };

  const loadBrands = async () => {
    setLoading(true);
    fetchBrands(page, pageSize)
      .then((res) => {
        setBrands(res.data);
        setTotal(res.total);
      })
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    if (activeTab === "brands") {
      loadBrands();
    }
  }, [activeTab, page, pageSize]);

  // --- СОЗДАНИЕ / ОБНОВЛЕНИЕ ---
  const onSubmit = async (data: IBrandResponse) => {
    try {
      if (isEditMode && editingBrand) {
        const response = await apiService.put<IBrandResponse>(
          `/v1/brands/${editingBrand.id}`,
          data
        );
        if (response.success) {
          toast.success("Бренд успешно обновлён!");
          setIsModalOpen(false);
          setEditingBrand(null);
          reset();
          loadBrands();
        } else {
          toast.error(response.error?.detail || "Ошибка при обновлении бренда");
        }
      } else {
        const response = await apiService.post<IBrandResponse>(
          "/v1/brands",
          data
        );
        if (response.success) {
          toast.success("Бренд успешно создан!");
          setIsModalOpen(false);
          reset();
          loadBrands();
        } else {
          toast.error(response.error?.detail || "Ошибка при создании бренда");
        }
      }
    } catch (e) {
      console.error(e);
      toast.error("Ошибка при сохранении бренда");
    }
  };

  // --- УДАЛЕНИЕ ---
  const handleDeleteBrand = async (brand: IBrandResponse) => {
    modal.confirm({
      title: `Удалить бренд "${brand.name}"?`,
      icon: <ExclamationCircleOutlined />,
      content: "Это действие нельзя будет отменить.",
      okText: "Удалить",
      okType: "danger",
      cancelText: "Отмена",
      async onOk() {
        try {
          const response = await apiService.delete(`/v1/brands/${brand.id}`);
          if (response.success) {
            toast.success("Бренд удалён");
            loadBrands();
          } else {
            toast.error(response.error?.detail || "Ошибка при удалении");
          }
        } catch (e) {
          console.error(e);
          toast.error("Ошибка при удалении бренда");
        }
      },
    });
  };

  // --- ОТКРЫТИЕ МОДАЛКИ ДЛЯ РЕДАКТИРОВАНИЯ ---
  const openEditModal = (brand: IBrandResponse) => {
    setIsEditMode(true);
    setEditingBrand(brand);
    setIsModalOpen(true);
    setValue("id", brand.id);
    setValue("name", brand.name);
    setValue("description", brand.description || "");
  };

  // --- КОЛОНКИ ТАБЛИЦЫ ---
  const columns: ColumnsType<IBrandResponse> = [
    {
      title: "Название",
      dataIndex: "name",
      key: "name",
      width: "30%",
    },
    {
      title: "Описание",
      dataIndex: "description",
      key: "description",
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
            onClick={() => handleDeleteBrand(record)}
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
        <h2 className="text-lg font-semibold">Брэнды</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => {
            setIsModalOpen(true);
            setIsEditMode(false);
            setEditingBrand(null);
            reset();
          }}
        >
          Добавить бренд
        </Button>
      </div>

      {loading ? (
        <div className="flex justify-center items-center py-10">
          <Spin />
        </div>
      ) : (
        <>
          <Table
            dataSource={brands}
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

      {/* Modal для создания / редактирования бренда */}
      <Modal
        title={isEditMode ? "Редактировать бренд" : "Добавить бренд"}
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          setEditingBrand(null);
          reset();
        }}
        footer={null}
      >
        <Form layout="vertical" onFinish={handleSubmit(onSubmit)}>
          <Controller
            name="id"
            control={control}
            render={({ field }) => <input type="hidden" {...field} />}
          />

          <Form.Item label="Название" required>
            <Controller
              name="name"
              control={control}
              rules={{ required: "Введите название" }}
              render={({ field, fieldState }) => (
                <>
                  <Input {...field} placeholder="Название бренда" />
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
                <Input.TextArea {...field} placeholder="Описание" rows={3} />
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
