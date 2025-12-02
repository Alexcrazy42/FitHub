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
  Input,
  DatePicker,
  TimePicker,
  Select,
  Space,
  Popconfirm,
  Checkbox,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { useForm, Controller, SubmitHandler } from "react-hook-form";
import dayjs, { Dayjs } from "dayjs";
import "dayjs/locale/ru";
import { IAddOrUpdateGroupTrainingRequest, IGroupTrainingResponse } from "../../../types/trainings"
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import { toast } from "react-toastify";


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

type Trainer = { value: string; color: string };

const sampleTrainers: Trainer[] = [
  { value: "Иван", color: "#60A5FA" },
  { value: "Мария", color: "#34D399" },
  { value: "Ольга", color: "#F59E0B" },
];

type ScheduleFormValues = {
  title: string;
  trainer: string;
  date: Dayjs;
  timeStart: Dayjs;
  timeEnd: Dayjs;
};


export const Schedule: React.FC = () => {
  const [events, setEvents] = useState<IGroupTrainingResponse[]>([]);

  const [currentView, setCurrentView] = useState<"month" | "week" | "day">("week");
  const [currentDate, setCurrentDate] = useState(new Date());
  const [selectedTrainer, setSelectedTrainer] = useState<string | null>(null);
  const [onlyShowSelected, setOnlyShowSelected] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingEvent, setEditingEvent] = useState<IGroupTrainingResponse | null>(null);
  const apiService = useApiService();

  const { control, handleSubmit, reset } = useForm<ScheduleFormValues>({
    defaultValues: {
      title: "",
      trainer: "",
      date: dayjs(),
      timeStart: dayjs().hour(10).minute(0),
      timeEnd: dayjs().hour(11).minute(0),
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

  const updateTraining = async (id: string, request  :IAddOrUpdateGroupTrainingRequest) : Promise<IGroupTrainingResponse> => {
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
  }, []);


  const trainerOptions = useMemo(
    () => sampleTrainers.map((trainer) => ({ label: trainer.value, value: trainer.value })),
    []
  );

  const openCreateModal = ({ start, end }: SlotInfo) => {
      setEditingEvent(null);
      reset({
        title: "",
        trainer: "",
        date: dayjs(start),
        timeStart: dayjs(start),
        timeEnd: dayjs(end),
      });
      setIsModalOpen(true);
    };

  const openEditModal = (ev: IGroupTrainingResponse) => {
    setEditingEvent(ev);
    reset({
      title: ev.baseGroupTraining.name,
      trainer: ev.trainer.user.email,
      date: dayjs(ev.startTime),
      timeStart: dayjs(ev.startTime),
      timeEnd: dayjs(ev.endTime),
    });
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingEvent(null);
  };


  const onSubmit: SubmitHandler<ScheduleFormValues> = async (values : ScheduleFormValues) => {
    const start = dayjs(values.date)
      .hour(values.timeStart.hour())
      .minute(values.timeStart.minute())
      .toDate();

    const end = dayjs(values.date)
      .hour(values.timeEnd.hour())
      .minute(values.timeEnd.minute())
      .toDate();


    if (editingEvent) {
      const request : IAddOrUpdateGroupTrainingRequest = {
        baseGroupTrainingId: editingEvent.baseGroupTraining.id,
        gymId: editingEvent.gym.id,
        trainerId: editingEvent.trainer.id,
        startTime: values.timeStart.toDate(),
        endTime: values.timeEnd.toDate(),
        isActive: editingEvent.isActive
      };

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
      

      // const color =
      //   sampleTrainers.find((t) => t.value === values.trainer)?.color ??
      //   "#60A5FA";

      // const newEvent: IGroupTrainingResponse = {
      //   id: String(Date.now()),
      //   title: values.title,
      //   trainer: values.trainer,
      //   start,
      //   end,
      //   color,
      // };


      // try {
      //   const body = { start, end, status: 'Start' };
      //   const res = await fetch(`http://localhost:5209/api/Test/test`, {
      //     method: "POST",
      //     headers: { "Content-Type": "application/json" },
      //     body: JSON.stringify(body),
      //   });
      //   console.log("POST body:", JSON.stringify(newEvent, null, 2));
      //   const data = await res.json().catch(() => null);
      //   console.log("Response:", data);
      //   setEvents((prev) => [...prev, newEvent]);
      // } catch (err) {
      //   console.error("Ошибка создания:", err);
      // }
    }

    closeModal();
  };


  const handleEventDrop = async ({ event, start, end }: EventDropArg<IGroupTrainingResponse>) => {
    const request : IAddOrUpdateGroupTrainingRequest = {
        baseGroupTrainingId: null,
        gymId: null,
        trainerId: null,
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

  const handleDeleteEvent = (id: string) => {
    console.log("📡 DELETE /api/training/delete");
    console.log("BODY:", { id });

    setEvents((prev) => prev.filter((e) => e.id !== id));
    closeModal();
  };

  const filteredEvents = useMemo(() => {
    if (!selectedTrainer) return events;
    return onlyShowSelected
      ? events.filter((e) => e.trainer.id === selectedTrainer)
      : events;
  }, [events, selectedTrainer, onlyShowSelected]);

  const eventStyleGetter = (event: IGroupTrainingResponse) => {
    const isHighlighted = selectedTrainer && event.trainer.id === selectedTrainer;
    const baseColor = "#60A5FA";
    return {
      style: {
        backgroundColor: isHighlighted
          ? baseColor
          : selectedTrainer
          ? "#E6E6E6"
          : baseColor,
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
          <Select
            placeholder="Выделить тренера"
            allowClear
            value={selectedTrainer ?? undefined}
            style={{ width: 180 }}
            options={trainerOptions}
            onChange={(v) => setSelectedTrainer(v ?? null)}
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
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
          <Controller
            name="title"
            control={control}
            rules={{ required: true }}
            render={({ field }) => <Input {...field} placeholder="Название" />}
          />

          <Controller
            name="trainer"
            control={control}
            rules={{ required: true }}
            render={({ field }) => (
              <Select {...field} options={trainerOptions} placeholder="Тренер" />
            )}
          />

          <Controller
            name="date"
            control={control}
            rules={{ required: true }}
            render={({ field }) => (
              <DatePicker
                className="w-full"
                format="DD.MM.YYYY"
                {...field}
                value={field.value}
              />
            )}
          />

          <div className="grid grid-cols-2 gap-3">
            <Controller
              name="timeStart"
              control={control}
              render={({ field }) => (
                <TimePicker
                  className="w-full"
                  format="HH:mm"
                  {...field}
                  value={field.value}
                />
              )}
            />
            <Controller
              name="timeEnd"
              control={control}
              render={({ field }) => (
                <TimePicker
                  className="w-full"
                  format="HH:mm"
                  {...field}
                  value={field.value}
                />
              )}
            />
          </div>

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
