import React, { useEffect, useState } from "react";
import { Table, Pagination, Drawer, Button, Card, TabsProps, Tabs } from "antd";
import { IGymResponse, IGymZoneResponse, IUpdateGymRequest, IUpdateGymZoneRequest } from "../../../types/gyms";
import { ListResponse } from "../../../types/common";
import { toast } from "react-toastify";
import { useApiService } from "../../../api/useApiService";
import { GymForm } from "./GymForm";

export const Gyms: React.FC = () => {
  const [gymLoading, setGymZoneLoading] = useState(false);
  const [gyms, setGyms] = useState<IGymResponse[]>([]);
  const [gymZones, setGymZones] = useState<IGymZoneResponse[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalGymCount, setTotalGymCount] = useState(0);
  const [gymDrawerVisible, setGymDrawerVisible] = useState(false);
  const [gymZoneDrawerVisible, setGymZoneDrawerVisible] = useState(false);
  const [selectedGym, setSelectedGym] = useState<IGymResponse | null>(null);
  const [selectedGymZone, setSelectedGymZone] = useState<IGymZoneResponse | null>(null);
  const [gymFormLoading, setFormLoading] = useState(false);
  const [gymZoneFormLoading, setGymFormLoading] = useState(false);
  const [activeTab, setActiveTab] = useState('gyms');
  
  const apiService = useApiService();

  const fetchGyms = async () => {
    try {
      setGymZoneLoading(true);
      const response = await apiService.get<ListResponse<IGymResponse>>(
        `v1/gyms?PageNumber=${currentPage}&PageSize=${pageSize}`
      );
      
      if (response.success && response.data) {
        setGyms(response.data.items);
        const count = response.data.totalItems;
        if(count) {
          setTotalGymCount(count);
        }
      }
    } catch (error) {
      toast.error("Ошибка при загрузке спортзалов");
    } finally {
      setGymZoneLoading(false);
    }
  };

  const fetchGymZones = async () => {
    try {
      setGymZoneLoading(true);
      const response = await apiService.get<ListResponse<IGymZoneResponse>>(`/v1/gym-zones`);
      
      if (response.success && response.data) {
        setGymZones(response.data.items);
      }
    } catch (error) {
      toast.error("Ошибка при загрузке зон");
    } finally {
      setGymZoneLoading(false);
    }
  };

  const handleGymDrawerClose = () => {
    setGymDrawerVisible(false);
    setSelectedGym(null);
  };

  const handleGymZoneDrawerClose = () => {
    setGymZoneDrawerVisible(false);
    setSelectedGymZone(null);
  }

  const getGym = async (gym : IGymResponse) : Promise<IGymResponse> => {
    const response = await apiService.get<IGymResponse>(`v1/gyms/${gym.id}`);
    if(response.success && response.data) {
      return response.data;
    }
    return gym;
  }

  const refreshGym = async (gym : IGymResponse) : Promise<IGymResponse> => {
    const currentGym = await getGym(gym);
    setSelectedGym(currentGym);
    return currentGym;
  };

  const handleGymSave = async (values: IUpdateGymRequest): Promise<IGymResponse> => {
    try {
      setFormLoading(true);
      if (selectedGym) {
        const response = await apiService.put<IGymResponse>(`v1/gyms`, values);
        if (response.success && response.data) {
          toast.success("Спортзал успешно обновлен");
          fetchGyms();
          handleGymDrawerClose();
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

  const handleGymZoneSave = async(values: IUpdateGymZoneRequest) : Promise<IGymZoneResponse> => {
    try {
      setFormLoading(true);
      if (selectedGym) {
        const response = await apiService.put<IGymZoneResponse>(`v1/gyms-zones`, values);
        if (response.success && response.data) {
          toast.success("Зона успешно обновлена");
          fetchGymZones();
          handleGymZoneDrawerClose();
          return response.data;
        } else {
          throw new Error(response.error?.detail || "Ошибка обновления");
        }
      }
      throw new Error("Зона не выбрана");
    } catch (error) {
      toast.error("Ошибка при сохранении");
      throw error;
    } finally {
      setFormLoading(false);
    }
  }

  const handleGymPageChange = (page: number, size?: number) => {
    setCurrentPage(page);
    if (size) {
      setPageSize(size);
    }
  };

  const handleGymRowClick = async (gym: IGymResponse) => {
      const currentGym = await getGym(gym);
      setSelectedGym(currentGym);
      setGymDrawerVisible(true);
  };

  const handleGymZoneRowClick = async (gymZone : IGymZoneResponse) => {
    setSelectedGymZone(gymZone);
    setGymZoneDrawerVisible(true);
  }


  useEffect(() => {
    fetchGyms();
  }, [currentPage, pageSize]);

  useEffect(() => {
    if(activeTab == 'zones') {
      fetchGymZones();
    }
  }, [activeTab])

  const gymTableColumns = [
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
    }
  ];

  const gymZoneTableColumns = [
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
    }
  ];

  const items: TabsProps['items'] = [
    {
      key: 'gyms',
      label: 'Залы',
      children: (
        <>
          <Button type="primary" onClick={fetchGyms}>
            Обновить
          </Button>
          <Table
            columns={gymTableColumns}
            dataSource={gyms}
            rowKey="id"
            loading={gymLoading}
            pagination={false}
            scroll={{ x: 800 }}
            className="mb-4"
            onRow={(record) => ({
              onClick: () => handleGymRowClick(record),
              style: { cursor: 'pointer' },
            })}
          />

          <div className="flex justify-end">
            <Pagination
              current={currentPage}
              pageSize={pageSize}
              total={totalGymCount}
              onChange={handleGymPageChange}
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
            onClose={handleGymDrawerClose}
            open={gymDrawerVisible}
            width={600}
            destroyOnClose
          >
            {selectedGym && (
              <GymForm
                gym={selectedGym}
                onSave={handleGymSave}
                onCancel={handleGymDrawerClose}
                loading={gymFormLoading}
                refresh={refreshGym}
              />
            )}
          </Drawer>
        </>
      ),
    },
    {
      key: 'zones',
      label: 'Зоны',
      children: (
        <>
        <Button type="primary" onClick={fetchGyms}>
            Обновить
          </Button>
          <Table
            columns={gymZoneTableColumns}
            dataSource={gymZones}
            rowKey="id"
            loading={gymLoading}
            pagination={false}
            scroll={{ x: 800 }}
            className="mb-4"
            onRow={(record) => ({
              onClick: () => handleGymZoneRowClick(record),
              style: { cursor: 'pointer' },
            })}
          />

          <div className="flex justify-end">
            <Pagination
              current={currentPage}
              pageSize={pageSize}
              total={totalGymCount}
              onChange={handleGymPageChange}
              showSizeChanger
              showQuickJumper
              showTotal={(total, range) =>
                `Показано ${range[0]}-${range[1]} из ${total} записей`
              }
              pageSizeOptions={['10', '20', '50', '100']}
            />
          </div>

          {/* <Drawer
            title={`Редактирование спортзала: ${selectedGym?.name}`}
            placement="right"
            onClose={handleGymDrawerClose}
            open={gymDrawerVisible}
            width={600}
            destroyOnClose
          >
            {selectedGym && (
              <GymForm
                gym={selectedGym}
                onSave={handleGymSave}
                onCancel={handleGymDrawerClose}
                loading={gymFormLoading}
                refresh={refreshGym}
              />
            )}
          </Drawer> */}
        </>
      ),
    },
  ];

   return (
    <div className="p-6">
      <Card
        title="Спортзалы"
        className="shadow-sm"
      >
        <Tabs
          activeKey={activeTab}
          onChange={setActiveTab}
          items={items}
          type="card"
        />
      </Card>
    </div>
  );
};