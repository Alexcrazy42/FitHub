import React, { useEffect, useState } from "react";
import { Table, Pagination, Drawer, Button, Space, Card } from "antd";
import { EditOutlined } from "@ant-design/icons";
import { IGymResponse, IUpdateGymRequest } from "../../../types/gyms";
import { ListResponse } from "../../../types/common";
import { toast } from "react-toastify";
import { useApiService } from "../../../api/useApiService";
import { GymForm } from "./GymForm";

export const Gyms: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [gyms, setGyms] = useState<IGymResponse[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [selectedGym, setSelectedGym] = useState<IGymResponse | null>(null);
  const [formLoading, setFormLoading] = useState(false);
  
  const apiService = useApiService();

  const fetchGyms = async () => {
    try {
      setLoading(true);
      const response = await apiService.get<ListResponse<IGymResponse>>(
        `v1/gyms?PageNumber=${currentPage}&PageSize=${pageSize}`
      );
      
      if (response.success && response.data) {
        setGyms(response.data.items);
        const count = response.data.totalItems;
        if(count) {
          setTotalItems(count);
        }
        
      } else {
        toast.error(response.error?.detail || "Ошибка загрузки данных");
      }
    } catch (error) {
      console.error('Ошибка запроса:', error);
      toast.error("Ошибка при загрузке спортзалов");
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (gym: IGymResponse) => {
    setSelectedGym(gym);
    setDrawerVisible(true);
  };

  const handleDrawerClose = () => {
    setDrawerVisible(false);
    setSelectedGym(null);
  };



  const handleSave = async (values: IUpdateGymRequest): Promise<IGymResponse> => {
  try {
    setFormLoading(true);
    if (selectedGym) {
      const response = await apiService.put<IGymResponse>(`v1/gyms`, values);
      if (response.success && response.data) {
        toast.success("Спортзал успешно обновлен");
        fetchGyms();
        handleDrawerClose();
        return response.data;
      } else {
        throw new Error(response.error?.detail || "Ошибка обновления");
      }
    }
    throw new Error("Спортзал не выбран");
  } catch (error) {
    toast.error("Ошибка при сохранении");
    throw error;
  } finally {
    setFormLoading(false);
  }
};

  const handlePageChange = (page: number, size?: number) => {
    setCurrentPage(page);
    if (size) {
      setPageSize(size);
    }
  };

  const handlePhotoUpload = async (gymId: string, file: File) => {
  try {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await apiService.post<IGymResponse>(
      `v1/gyms/${gymId}/photo`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    
    if (response.success && response.data) {
      // Обновляем данные в списке
      fetchGyms();
      return response.data;
    } else {
      throw new Error(response.error?.detail || "Ошибка загрузки фото");
    }
  } catch (error) {
    console.error('Ошибка загрузки фото:', error);
    throw error;
  }
};

  useEffect(() => {
    fetchGyms();
  }, [currentPage, pageSize]);

  // Колонки таблицы
  const columns = [
    // {
    //   title: 'ID',
    //   dataIndex: 'id',
    //   key: 'id',
    //   width: 100,
    //   render: (id: string) => (
    //     <span className="font-mono text-xs">{id.slice(0, 8)}...</span>
    //   ),
    // },
    {
      title: 'Название',
      dataIndex: 'name',
      key: 'name',
      render: (name: string) => (
        <span className="font-medium">{name}</span>
      ),
    },
    {
      title: 'Описание',
      dataIndex: 'description',
      key: 'description',
      render: (description: string) => (
        <span className="text-gray-600">{description}</span>
      ),
    },
    {
      title: 'Изображение',
      dataIndex: 'imageUrl',
      key: 'imageUrl',
      render: (imageUrl: string | null) => (
        imageUrl ? (
          <img 
            src={imageUrl} 
            alt="Gym" 
            className="w-12 h-12 object-cover rounded"
          />
        ) : (
          <span className="text-gray-400">Нет изображения</span>
        )
      ),
    },
    {
      title: 'Действия',
      key: 'actions',
      width: 100,
      render: (_: any, record: IGymResponse) => (
        <Space>
          <Button
            type="primary"
            icon={<EditOutlined />}
            size="small"
            onClick={() => handleEdit(record)}
          >
            Редактировать
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div className="p-6">
      <Card 
        title="Спортзалы" 
        className="shadow-sm"
        extra={
          <Button type="primary" onClick={fetchGyms}>
            Обновить
          </Button>
        }
      >
        <Table
          columns={columns}
          dataSource={gyms}
          rowKey="id"
          loading={loading}
          pagination={false}
          scroll={{ x: 800 }}
          className="mb-4"
        />
        
        <div className="flex justify-end">
          <Pagination
            current={currentPage}
            pageSize={pageSize}
            total={totalItems}
            onChange={handlePageChange}
            showSizeChanger
            showQuickJumper
            showTotal={(total, range) => 
              `Показано ${range[0]}-${range[1]} из ${total} записей`
            }
            pageSizeOptions={['10', '20', '50', '100']}
          />
        </div>

        <Drawer
          title={`Редактирование спортзала: ${selectedGym?.name}`}
          placement="right"
          onClose={handleDrawerClose}
          open={drawerVisible}
          width={600}
          destroyOnClose
        >
          {selectedGym && (
            <GymForm
              gym={selectedGym}
              onSave={handleSave}
              onPhotoUpload={handlePhotoUpload}
              onCancel={handleDrawerClose}
              loading={formLoading}
            />
          )}
        </Drawer>
      </Card>
    </div>
  );
};