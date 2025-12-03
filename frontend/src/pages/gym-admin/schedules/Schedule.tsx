import React, { useEffect, useMemo, useState } from "react";
import "react-big-calendar/lib/css/react-big-calendar.css";
import "react-big-calendar/lib/addons/dragAndDrop/styles.css";
import {
  Calendar as RBC,
  dateFnsLocalizer,
  Event as RBCEvent,
  SlotInfo
} from "react-big-calendar";
import withDragAndDrop, {
  EventResizeArg,
  EventDropArg,
} from "react-big-calendar/lib/addons/dragAndDrop";
import { format, parse, startOfWeek, getDay } from "date-fns";
import { ru } from "date-fns/locale";
import {
  Button,
  Modal,
  DatePicker,
  TimePicker,
  Space,
  Popconfirm,
  Checkbox,
  Switch
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { useForm, Controller, SubmitHandler } from "react-hook-form";
import dayjs from "dayjs";
import "dayjs/locale/ru";
import { IAddOrUpdateGroupTrainingRequest, IBaseGroupTraining, IGroupTrainingResponse } from "../../../types/trainings"
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import { toast } from "react-toastify";
import { useAuth } from "../../../context/useAuth";
import { UniversalSelect } from "../../../components/UniversalSelector/UniversalSelect";
import { IGymResponse } from "../../../types/gyms";
import { ITrainerResponse } from "../../../types/auth";


dayjs.locale("ru");

const locales = { ru };
const localizer = dateFnsLocalizer({
  format: (date: string | number | Date, formatStr: string) => format(date, formatStr, { locale: ru }),
  parse: (value: string, formatStr: string) =>
    parse(value, formatStr, new Date(), { locale: ru }),
  startOfWeek: () => startOfWeek(new Date(), { weekStartsOn: 1 }),
  getDay,
  locales,
});

const DnDCalendar = withDragAndDrop(RBC);

export const Schedule: React.FC = () => {
  const [events, setEvents] = useState<IGroupTrainingResponse[]>([]);
  const { currentGym } = useAuth();

  const [currentView, setCurrentView] = useState<"month" | "week" | "day">("week");
  const [currentDate, setCurrentDate] = useState(new Date());
  const [dateRange, setDateRange] = useState<{ start: Date; end: Date } | null>(null);
  const [selectedTrainer, setSelectedTrainer] = useState<string | null>(null);
  const [onlyShowSelected, setOnlyShowSelected] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingEvent, setEditingEvent] = useState<IGroupTrainingResponse | null>(null);
  const apiService = useApiService();

  const { control, handleSubmit, reset } = useForm<IAddOrUpdateGroupTrainingRequest>({
    defaultValues: {
      baseGroupTrainingId: null,
      gymId: null,
      trainerId: null,
      date: null,
      startTime: dayjs().hour(10).minute(0).toDate(),
      endTime: dayjs().hour(11).minute(0).toDate(),
      isActive: null
    },
  });


  const fetchAll = async () => {
    try {
      const response = await apiService.get<ListResponse<IGroupTrainingResponse>>(`v1/group-trainings?PageNumber=1&PageSize=100`);

      if(response.success && response.data) {
        const items = response.data.items ?? [];
        const normalized = items.map(x => ({
          ...x,
          startTime: new Date(x.startTime),
          endTime: new Date(x.endTime)
        }));

        setEvents(normalized);
      } else {
        toast.error(response.error?.detail ?? 'Не удалось загрузить тренировки!');
        setEvents([]);
      }
    } catch(err)  {
      console.error('fetch trainings errors: ', err);
      toast.error("Ошибка при загрузке тренеров!");
      setEvents([]);
    }
  }

  const createTraining = async (request : IAddOrUpdateGroupTrainingRequest) : Promise<IGroupTrainingResponse> => {
    const response = await apiService.post<IGroupTrainingResponse>('/v1/group-trainings', request);
    if (response.success && response.data) {
      return response.data;
    } else {
      toast.error(response.error?.detail ?? 'Ошибка при создании тренировки!');
      throw new Error(response.error?.detail);
    }
  }

  const updateTraining = async (id: string, request : IAddOrUpdateGroupTrainingRequest) : Promise<IGroupTrainingResponse> => {
    const response = await apiService.put<IGroupTrainingResponse>(`/v1/group-trainings/${id}`, request);
    if (response.success && response.data) {
      return response.data;
    } else {
      toast.error(response.error?.detail ?? 'Ошибка при создании тренировки!');
      throw new Error(response.error?.detail);
    }
  }

  useEffect(() => {
    fetchAll();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);


  const openCreateModal = ({ start, end }: SlotInfo) => {
      setEditingEvent(null);
      reset({
        baseGroupTrainingId: null,
        gymId: null,
        trainerId: null,
        startTime: start,
        endTime: end,
        isActive: null
      });
      setIsModalOpen(true);
    };

  const openEditModal = (ev: IGroupTrainingResponse) => {
    setEditingEvent(ev);
    reset({
        baseGroupTrainingId: ev.baseGroupTraining.id,
        gymId: ev.gym.id,
        trainerId: ev.trainer.id,
        date: dayjs(ev.startTime),
        startTime: ev.startTime,
        endTime: ev.endTime,
        isActive: ev.isActive
      });
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingEvent(null);
  };


  const onSubmit: SubmitHandler<IAddOrUpdateGroupTrainingRequest> = async (values : IAddOrUpdateGroupTrainingRequest) => {
    const request : IAddOrUpdateGroupTrainingRequest = {
        baseGroupTrainingId: values.baseGroupTrainingId,
        gymId: values.gymId,
        trainerId: values.trainerId,
        date: null,
        startTime: values.startTime,
        endTime: values.endTime,
        isActive: values.isActive
      };
    
    if (editingEvent) {
      const updatedTraining = await updateTraining(editingEvent.id, request);
      toast.success("Тренировка успешно обновлена!");
      setEditingEvent(null);

      const normalizedUpdatedTraining = {
          ...updatedTraining,
          startTime: new Date(updatedTraining.startTime),
          endTime: new Date(updatedTraining.endTime)
        };

      setEvents((prev) =>
        prev.map((ev) => (ev.id === editingEvent.id ? normalizedUpdatedTraining : ev))
      );
    }
    else {

      const createdTraining = await createTraining(request);
      toast.success("Тренировка успешно создана!");
      const normalizedCreatedTraining = {
        ...createdTraining,
        startTime: new Date(createdTraining.startTime),
        endTime: new Date(createdTraining.endTime)
      };

      setEvents((prev) => [...prev, normalizedCreatedTraining]);
    }

    closeModal();
  };


  const handleEventDrop = async ({ event, start, end }: EventDropArg<IGroupTrainingResponse>) => {
    const request : IAddOrUpdateGroupTrainingRequest = {
        baseGroupTrainingId: null,
        gymId: null,
        trainerId: null,
        date: null,
        startTime: start,
        endTime: end,
        isActive: null
      };

      const updatedTraining = await updateTraining(event.id, request);
      toast.success("Тренировка успешно обновлена!");
      setEditingEvent(null);

      const normalizedUpdatedTraining = {
          ...updatedTraining,
          startTime: new Date(updatedTraining.startTime),
          endTime: new Date(updatedTraining.endTime)
        };

      setEvents((prev) =>
        prev.map((ev) => (ev.id === event.id ? normalizedUpdatedTraining : ev))
      );
  };


  const handleEventResize = async ({
    event,
    start,
    end,
  }: EventResizeArg<IGroupTrainingResponse>) => {
    const request : IAddOrUpdateGroupTrainingRequest = {
        baseGroupTrainingId: null,
        gymId: null,
        trainerId: null,
        date: null,
        startTime: start,
        endTime: end,
        isActive: null
      };

      const updatedTraining = await updateTraining(event.id, request);
      toast.success("Тренировка успешно обновлена!");
      setEditingEvent(null);

      const normalizedUpdatedTraining = {
          ...updatedTraining,
          startTime: new Date(updatedTraining.startTime),
          endTime: new Date(updatedTraining.endTime)
        };

      setEvents((prev) =>
        prev.map((ev) => (ev.id === event.id ? normalizedUpdatedTraining : ev))
      );
  };

  const handleDeleteEvent = async (id: string) => {
    try {
      const response = await apiService.delete<never>(`/v1/group-trainings/${id}`);
      if(response.success) {
        toast.success("Тренировка успешно удалена!");
      }
      setEvents((prev) => prev.filter((ev) => ev.id !== id));
    } catch(err) {
      console.error(err);
      toast.error("Произошла ошибка при удалении зала!");
    }
    closeModal();
  };

  const filteredEvents = useMemo(() => {
    if (!selectedTrainer) return events;
    return onlyShowSelected
      ? events.filter((e) => e.trainer.id === selectedTrainer)
      : events;
  }, [events, selectedTrainer, onlyShowSelected]);

  const getTrainerColor = (trainerId: string): string => {
    let hash = 0;
    for (let i = 0; i < trainerId.length; i++) {
      hash = trainerId.charCodeAt(i) + ((hash << 5) - hash);
    }
    
    // Палитра приятных цветов для календаря
    const colors = [
      '#60A5FA', // blue
      '#34D399', // green
      '#F59E0B', // amber
      '#EC4899', // pink
      '#8B5CF6', // purple
      '#10B981', // emerald
      '#F97316', // orange
      '#06B6D4', // cyan
      '#EF4444', // red
      '#14B8A6', // teal
      '#A78BFA', // violet
      '#FB923C', // orange-400
    ];
    
    // Выбираем цвет на основе хеша
    const index = Math.abs(hash) % colors.length;
    return colors[index];
  };

  const eventStyleGetter = (event: IGroupTrainingResponse) => {
    const isHighlighted = selectedTrainer && event.trainer.id === selectedTrainer;
    const trainerColor = getTrainerColor(event.trainer.id);
    
    return {
      style: {
        backgroundColor: isHighlighted
          ? trainerColor
          : selectedTrainer
          ? "#E6E6E6"
          : trainerColor,
        border: "1px solid rgba(0,0,0,0.08)",
        color: "#0f172a",
        paddingLeft: 6,
        borderRadius: 4,
        opacity: selectedTrainer && !isHighlighted ? 0.6 : 1,
      },
    };
  };

  const EventRenderer: React.FC<{ event: IGroupTrainingResponse }> = ({ event }) => (
    <div className="flex items-center justify-between">
      <div className="text-xs">
        <div className="font-medium truncate">{event.baseGroupTraining.name}</div>
        <div className="text-[11px] text-gray-700 truncate">{event.trainer.user.surname} {event.trainer.user.name}</div>
      </div>
    </div>
  );

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold">Расписание</h1>
        <div className="flex items-center gap-3">
          <UniversalSelect
            mode="infinite"
            pageSize={20}
            fetchPage={async (page, pageSize) => {
              const gymId = currentGym?.id;
              const params = new URLSearchParams({
                PageNumber: page.toString(),
                PageSize: pageSize.toString(),
                ...(gymId && { GymId: gymId.toString() })
              });

              const query = `/v1/trainers?${params}`;
              const response = await apiService.get<ListResponse<ITrainerResponse>>(query);
              
              if (response.success && response.data) {
                const items = gymId 
                  ? response.data.items.filter((u: ITrainerResponse) => 
                      u.gyms.some(gym => gym.id === gymId)
                    )
                  : response.data.items;
                
                return {
                  items: items.map((u: ITrainerResponse) => ({
                    value: u.id,
                    label: `${u.user.surname} ${u.user.name}`,
                  })),
                  hasMore: (response.data.totalPages ?? 0) > page
                };
              }
              
              return {
                items: [],
                hasMore: false
              };
            }}
            placeholder="Выделить тренера"
            allowClear
            value={selectedTrainer ?? undefined}
            onChange={(v) => setSelectedTrainer((v as string) ?? null)}
            style={{ width: 180 }}
          />
          <Checkbox
            checked={onlyShowSelected}
            onChange={(e) => setOnlyShowSelected(e.target.checked)}
          >
            Только выбранный
          </Checkbox>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() =>
              openCreateModal({
                start: new Date(),
                end: dayjs().add(1, "hour").toDate(),
              })
            }
          >
            Создать
          </Button>
        </div>
      </div>

      <div style={{ height: "75vh" }}>
        <DnDCalendar
          views={["month", "week", "day"]}
          view={currentView}
          date={currentDate}
          onView={(view) => setCurrentView(view)}
          onNavigate={(date) => setCurrentDate(date)}
          // TODO доделать так, чтобы выдавались события толкьо по определенному залу + определенным датам
          onRangeChange={(range) => {
            if (Array.isArray(range)) {
              const rangeItem = {
                start: range[0],
                end: range[range.length - 1]
              }
              setDateRange(rangeItem);
              console.log(rangeItem)
            } else if (range.start && range.end) {
              setDateRange(range);
              console.log(range)
            }
          }}
          localizer={localizer}
          events={filteredEvents as RBCEvent[]}
          startAccessor="startTime"
          endAccessor="endTime"
          selectable
          resizable
          onSelectEvent={openEditModal}
          onSelectSlot={(slotInfo) => openCreateModal(slotInfo)}
          onEventDrop={handleEventDrop}
          onEventResize={handleEventResize}
          style={{ height: "100%" }}
          eventPropGetter={eventStyleGetter}
          components={{ event: EventRenderer }}
          defaultDate={new Date()}
          popup
          messages={{
            week: "Неделя",
            work_week: "Рабочая неделя",
            day: "День",
            month: "Месяц",
            previous: "Назад",
            next: "Вперёд",
            today: "Сегодня",
            agenda: "Повестка",
            noEventsInRange: "Нет событий",
            showMore: (total) => `+ ещё ${total}`,
          }}
        />
      </div>


      <Modal
        title={editingEvent ? "Редактировать тренировку" : "Создать тренировку"}
        open={isModalOpen}
        onCancel={closeModal}
        footer={null}
        destroyOnHidden
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium">Тип тренировки</label>
            <Controller
              name="baseGroupTrainingId"
              control={control}
              rules={{ required: 'Выберите тип тренировки' }}
              render={({ field, fieldState: { error } }) => (
                <>
                  <UniversalSelect
                    mode="infinite"
                    fetchPage={async (page, pageSize) => {
                      const response = await apiService.get<ListResponse<IBaseGroupTraining>>(
                        `/v1/base-group-trainings?PageNumber=${page}&PageSize=${pageSize}`
                      );
                      if(response.success && response.data) {
                        const items = response.data.items;
                        return {
                          items: items.map((u: IBaseGroupTraining) => ({
                            value: u.id,
                            label: u.name ?? "",
                            payload: u.description
                          })),
                          hasMore: (response.data.totalPages ?? 0) > page
                        };
                      }
                      return {
                        items: [],
                        hasMore: false
                      };
                    }}
                    placeholder="Выберите тип"
                    value={(field.value as string) ?? undefined}
                    onChange={field.onChange}
                  />
                  {error && <span className="text-red-500 text-xs">{error.message}</span>}
                </>
              )}
            />
        </div>


        <div className="flex flex-col gap-1">
            <label className="text-sm font-medium">Зал</label>
            <Controller
              name="gymId"
              control={control}
              rules={{ required: 'Выберите зал' }}
              render={({ field, fieldState: { error } }) => (
                <>
                  <UniversalSelect
                    mode="infinite"
                    fetchPage={async (page, pageSize) => {
                      const response = await apiService.get<ListResponse<IGymResponse>>(
                        `/v1/gyms?PageNumber=${page}&PageSize=${pageSize}`
                      );
                      if(response.success && response.data) {
                        const gymId = currentGym?.id;
                        const items = gymId 
                          ? response.data.items.filter((u: IGymResponse) => u.id === gymId)
                          : response.data.items;
                        return {
                          items: items.map((u: IGymResponse) => ({
                            value: u.id,
                            label: u.name,
                            payload: u.description
                          })),
                          hasMore: (response.data.totalPages ?? 0) > page
                        };
                      }
                      return {
                        items: [],
                        hasMore: false
                      };
                    }}
                    placeholder="Выберите зал" 
                    value={(field.value as string) ?? undefined}
                    onChange={field.onChange}
                  />
                  {error && <span className="text-red-500 text-xs">{error.message}</span>}
                </>
              )}
            />
        </div>


         <div className="flex flex-col gap-1">
            <label className="text-sm font-medium">Тренер</label>
            <Controller
              name="trainerId"
              control={control}
              rules={{ required: 'Выберите тренера' }}
              render={({ field, fieldState: { error } }) => (
                <>
                  <UniversalSelect
                    mode="infinite"
                    fetchPage={async (page, pageSize) => {
                      const gymId = currentGym?.id;
                      const params = new URLSearchParams({
                        PageNumber: page.toString(),
                        PageSize: pageSize.toString(),
                        ...(gymId && { GymId: gymId.toString() })
                      });

                      const query = `/v1/trainers?${params}`;
                      const response = await apiService.get<ListResponse<ITrainerResponse>>(query);
                      if(response.success && response.data) {
                        const gymId = currentGym?.id;
                        const items = gymId 
                          ? response.data.items.filter((u: ITrainerResponse) => 
                              u.gyms.some(gym => gym.id === gymId)
                            )
                          : response.data.items;
                        return {
                          items: items.map((u: ITrainerResponse) => ({
                            value: u.id,
                            label: `${u.user.surname} ${u.user.name}`,
                          })),
                          hasMore: (response.data.totalPages ?? 0) > page
                        };
                      }
                      return {
                        items: [],
                        hasMore: false
                      };
                    }}
                    placeholder="Выберите тренера" 
                    value={(field.value as string) ?? undefined}
                    onChange={field.onChange}
                  />
                  {error && <span className="text-red-500 text-xs">{error.message}</span>}
                </>
              )}
            />
        </div>

          {/* Date */}
          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium">Дата</label>
            <Controller
              name="date"
              control={control}
              rules={{ 
                required: 'Дата обязательна для заполнения' // или любой другой текст
              }}
              render={({ field, fieldState: { error } }) => (
                <>
                  <DatePicker
                    className="w-full"
                    format="DD.MM.YYYY"
                    value={field.value}
                    onChange={(date) => field.onChange(date)}
                    placeholder="Выберите дату" 
                    status={error ? 'error' : ''} // подсветка красным
                  />
                  {error && <span className="text-red-500 text-xs">{error.message}</span>}
                </>
              )}
            />
          </div>

          {/* Time */}
          <div className="grid grid-cols-2 gap-3">
            <div className="flex flex-col gap-1">
              <label className="text-sm font-medium">Начало</label>
              <Controller
                name="startTime"
                control={control}
                rules={{ 
                  required: 'Дата начала обязательна для заполнения' // или любой другой текст
                }}
                render={({ field, fieldState: { error } }) => (
                  <>
                  <TimePicker
                    className="w-full"
                    format="HH:mm"
                    value={field.value ? dayjs(field.value) : null}
                    placeholder="Время начала" 
                    onChange={(value) => field.onChange(value ? value.toDate() : null)}
                  />
                  {error && <span className="text-red-500 text-xs">{error.message}</span>}
                  </>
                )}
              />
            </div>

            <div className="flex flex-col gap-1">
              <label className="text-sm font-medium">Окончание</label>
              <Controller
                name="endTime"
                control={control}
                rules={{ 
                  required: 'Дата окончания обязательна для заполнения' // или любой другой текст
                }}
                render={({ field, fieldState: { error } }) => (
                  <>
                    <TimePicker
                      className="w-full"
                      format="HH:mm"
                      placeholder="Выберите конца" 
                      value={field.value ? dayjs(field.value) : null}
                      onChange={(value) => field.onChange(value ? value.toDate() : null)}
                    />
                    {error && <span className="text-red-500 text-xs">{error.message}</span>}
                  </>
                )}
              />
            </div>
          </div>

          {/* isActive */}
          <div className="flex items-center gap-3">
            <label className="text-sm font-medium">Активна</label>

            <Controller
              name="isActive"
              control={control}
              render={({ field }) => (
                <Switch
                  checked={field.value ?? false}
                  onChange={(checked) => field.onChange(checked)}
                />
              )}
            />
          </div>

          {/* Buttons */}
          <div className="flex justify-between items-center mt-4">
            <Space>
              <Button type="primary" htmlType="submit">
                {editingEvent ? "Сохранить" : "Создать"}
              </Button>

              {editingEvent && (
                <Popconfirm
                  title="Удалить тренировку?"
                  onConfirm={() => handleDeleteEvent(editingEvent.id)}
                >
                  <Button danger>Удалить</Button>
                </Popconfirm>
              )}
            </Space>

            <Button onClick={closeModal}>Отмена</Button>
          </div>

        </form>
      </Modal>
    </div>
  );
};

export default Schedule;
