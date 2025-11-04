import { Table, Pagination, Button, Modal, Form, Input } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useEffect, useState } from "react";
import { ICreateMuscleGroup, IMuscleGroup } from "../../../types/trainings";
import { ListResponse } from "../../../types/common";
import { useApiService } from "../../../api/useApiService";
import { toast } from "react-toastify";
import {
  PlusOutlined,
  EditOutlined,
  FolderAddOutlined,
  DeleteOutlined,
} from "@ant-design/icons";

interface MuscleGroupsTabProps {
  activeTab: string;
}

export const MuscleGroupsTab: React.FC<MuscleGroupsTabProps> = ({ activeTab }) => {
  const [muscleGroups, setMuscleGroups] = useState<IMuscleGroup[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(false);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<IMuscleGroup | null>(null);
  const [selectedParent, setSelectedParent] = useState<IMuscleGroup | null>(null);
  const [expandedRowKeys, setExpandedRowKeys] = useState<React.Key[]>([]);

  const [form] = Form.useForm();
  const apiService = useApiService();

  // =============================
  // CRUD методы
  // =============================
  const fetchMuscleGroups = async () => {
    setLoading(true);
    const response = await apiService.get<ListResponse<IMuscleGroup>>(
      `/v1/muscle-groups?PageNumber=${page}&PageSize=${pageSize}`
    );
    if (response.success) {
      setMuscleGroups(response.data?.items ?? []);
      setTotal(response.data?.totalItems ?? response.data?.items?.length ?? 0);
    }
    setLoading(false);
  };

  const createMuscleGroup = async (request: ICreateMuscleGroup) => {
    const response = await apiService.post<IMuscleGroup>("/v1/muscle-groups", request);
    if (response.success) {
      toast.success("Группа успешно создана!");

      // если создаем дочернюю — раскрываем родителя
      if (request.parentId) {
        setExpandedRowKeys((prev) => Array.from(new Set([...prev, request.parentId!])));
      }

      await fetchMuscleGroups();
    } else {
      toast.error(response.error?.detail);
    }
  };

  const updateMuscleGroup = async (id: string, request: ICreateMuscleGroup) => {
    const response = await apiService.put<IMuscleGroup>(
      `/v1/muscle-groups/${id}`,
      request
    );
    if (response.success) {
      toast.success("Группа успешно обновлена!");
      await fetchMuscleGroups();
    } else {
      toast.error(response.error?.detail);
    }
  };

  const deleteMuscleGroup = async (id: string) => {
    const response = await apiService.delete<IMuscleGroup>(`/v1/muscle-groups/${id}`);
    if (response.success) {
      toast.success("Группа успешно удалена!");
      await fetchMuscleGroups();
    } else {
      toast.error(response.error?.detail);
    }
  };

  useEffect(() => {
    fetchMuscleGroups();
  }, [activeTab, page, pageSize]);

  // =============================
  // Таблица
  // =============================
  const columns: ColumnsType<IMuscleGroup> = [
    {
      title: "Название",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Действия",
      key: "actions",
      render: (_, record) => (
        <>
          {record.parentId == null && (
              <Button
                  type="link"
                  icon={<FolderAddOutlined />}
                  onClick={() => {
                  setIsModalOpen(true);
                  setEditingGroup(null);
                  setSelectedParent(record);
                  form.resetFields();
                  }}
              >
                  Добавить
              </Button>
          )}
        

        <Button
          icon={<EditOutlined />}
          onClick={() => handleEdit(record)}
          type="link"
        >
          Редактировать
        </Button>

        <Button
          icon={<DeleteOutlined />}
          danger
          onClick={() => deleteMuscleGroup(record.id)}
          type="link"
        >
          Удалить
        </Button>
      </>
      ),
    },
  ];

  // =============================
  // Обработка модалки
  // =============================
  const handleAdd = () => {
    setEditingGroup(null);
    setSelectedParent(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleEdit = (record: IMuscleGroup) => {
    setEditingGroup(record);
    setSelectedParent(null);
    form.setFieldsValue({ name: record.name });
    setIsModalOpen(true);
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();

      const parentId =
        selectedParent && selectedParent.id !== editingGroup?.id
          ? selectedParent.id
          : null;

      const payload: ICreateMuscleGroup = {
        name: values.name,
        parentId: parentId ?? null
      };

      if (editingGroup) {
        await updateMuscleGroup(editingGroup.id, payload);
      } else {
        await createMuscleGroup(payload);
      }

      setIsModalOpen(false);
      form.resetFields();
      setSelectedParent(null);
    } catch (err) {
      console.error(err);
    }
  };

  // =============================
  // UI
  // =============================
  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-lg font-semibold">Группы мышц</h2>
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          Добавить группу
        </Button>
      </div>
      <Button onClick={fetchMuscleGroups}>
        Обновить
      </Button>

      <Table
        rowKey="id"
        columns={columns}
        dataSource={muscleGroups}
        loading={loading}
        pagination={false}
        expandable={{
          childrenColumnName: "childrens",
          defaultExpandAllRows: false,
          expandedRowKeys,
          onExpand: (expanded, record) => {
            setExpandedRowKeys((prev) =>
              expanded
                ? [...prev, record.id]
                : prev.filter((id) => id !== record.id)
            );
          },
          rowExpandable: (record) =>
            Array.isArray(record.childrens) && record.childrens.length > 0,
        }}
      />

      <div className="flex justify-end mt-4">
        <Pagination
          current={page}
          pageSize={pageSize}
          total={total}
          showSizeChanger
          showTotal={(total) => `Всего ${total} групп`}
          onChange={(p, size) => {
            setPage(p);
            setPageSize(size);
          }}
        />
      </div>

      {/* Модалка создания / редактирования */}
      <Modal
        title={editingGroup ? "Редактировать группу" : "Создать новую группу"}
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          setSelectedParent(null);
        }}
        onOk={handleSubmit}
        okText={editingGroup ? "Сохранить" : "Создать"}
      >
        <Form form={form} layout="vertical">
          <Form.Item
            label="Название группы"
            name="name"
            rules={[{ required: true, message: "Введите название" }]}
          >
            <Input placeholder="Например: Грудные мышцы" />
          </Form.Item>

          {selectedParent && (
            <p className="text-sm text-gray-500">
              Родительская группа: <b>{selectedParent.name}</b>
            </p>
          )}
        </Form>
      </Modal>
    </div>
  );
};
