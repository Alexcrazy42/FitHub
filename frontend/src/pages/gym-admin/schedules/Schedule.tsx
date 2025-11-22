import React, { useMemo, useState } from "react";
import "react-big-calendar/lib/css/react-big-calendar.css";
import "react-big-calendar/lib/addons/dragAndDrop/styles.css";

import {
  Calendar as RBC,
  dateFnsLocalizer,
  Event as RBCEvent,
  SlotInfo,
  View,
} from "react-big-calendar";
import withDragAndDrop, { EventResizeArg, EventDropArg } from "react-big-calendar/lib/addons/dragAndDrop";

import { format, parse, startOfWeek, getDay } from "date-fns";
import { ru } from "date-fns/locale";

import { Button, Modal, Input, DatePicker, TimePicker, Select, Space, Popconfirm, Checkbox } from "antd";
import { PlusOutlined } from "@ant-design/icons";

import { useForm, Controller, SubmitHandler } from "react-hook-form";
import dayjs, { Dayjs } from "dayjs";
import "dayjs/locale/ru";

dayjs.locale("ru");

// ---- date-fns локализатор ----
const locales = { ru };
const localizer = dateFnsLocalizer({
  format: (date, formatStr) => format(date, formatStr, { locale: ru }),
  parse: (value, formatStr) => parse(value, formatStr, new Date(), { locale: ru }),
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

export type TrainingEvent = {
  id: string;
  title: string;
  trainer: string;
  start: Date;
  end: Date;
  color?: string;
};

type ScheduleFormValues = {
  title: string;
  trainer: string;
  date: Dayjs;
  timeStart: Dayjs;
  timeEnd: Dayjs;
};

export const Schedule: React.FC = () => {
  const [events, setEvents] = useState<TrainingEvent[]>(() => {
    const base = dayjs().startOf("week").add(1, "day"); // понедельник
    return [
      { id: "1", title: "Йога", trainer: "Мария", start: base.hour(10).toDate(), end: base.hour(11).toDate(), color: "#34D399" },
      { id: "2", title: "Кроссфит", trainer: "Иван", start: base.add(1, "day").hour(18).toDate(), end: base.add(1, "day").hour(19).toDate(), color: "#60A5FA" },
      { id: "3", title: "Силовая", trainer: "Ольга", start: base.add(2, "day").hour(17).toDate(), end: base.add(2, "day").hour(18).toDate(), color: "#F59E0B" },
    ];
  });

  const [view, setView] = useState<View>("week");
  const [selectedTrainer, setSelectedTrainer] = useState<string | null>(null);
  const [onlyShowSelected, setOnlyShowSelected] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingEvent, setEditingEvent] = useState<TrainingEvent | null>(null);

  const { control, handleSubmit, reset } = useForm<ScheduleFormValues>({
    defaultValues: {
      title: "",
      trainer: "",
      date: dayjs(),
      timeStart: dayjs().hour(10).minute(0),
      timeEnd: dayjs().hour(11).minute(0),
    },
  });

  const trainerOptions = useMemo(() => sampleTrainers.map(t => ({ label: t.value, value: t.value })), []);

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

  const openEditModal = (ev: TrainingEvent) => {
    setEditingEvent(ev);
    reset({
      title: ev.title,
      trainer: ev.trainer,
      date: dayjs(ev.start),
      timeStart: dayjs(ev.start),
      timeEnd: dayjs(ev.end),
    });
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingEvent(null);
  };

  const onSubmit: SubmitHandler<ScheduleFormValues> = async (values) => {
    const start = dayjs(values.date)
      .hour(values.timeStart.hour())
      .minute(values.timeStart.minute())
      .toDate();

    const end = dayjs(values.date)
      .hour(values.timeEnd.hour())
      .minute(values.timeEnd.minute())
      .toDate();

    if (editingEvent) {
      const updatedEvent = { ...editingEvent, title: values.title, trainer: values.trainer, start, end };

      const body = { start, end, status: 'Test' };
      // --- PUT запрос на обновление ---
      try {
        const res = await fetch(`http://localhost:5209/api/Test/test`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(body),
        });
        console.log("PUT body:", JSON.stringify(updatedEvent, null, 2));
        const data = await res.json().catch(() => null);
        console.log("Response:", data);
      } catch (err) {
        console.error("Ошибка обновления:", err);
      }

      setEvents(prev => prev.map(ev => ev.id === editingEvent.id ? updatedEvent : ev));
    } else {
      const color = sampleTrainers.find(t => t.value === values.trainer)?.color ?? "#60A5FA";
      const newEvent: TrainingEvent = { id: String(Date.now()), title: values.title, trainer: values.trainer, start, end, color };

      // --- POST запрос на создание ---
      try {
        const body = { start, end, status: 'Start' };
        const res = await fetch(`http://localhost:5209/api/Test/test`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(body),
        });
        console.log("POST body:", JSON.stringify(newEvent, null, 2));
        const data = await res.json().catch(() => null);
        console.log("Response:", data);
      } catch (err) {
        console.error("Ошибка создания:", err);
      }

      setEvents(prev => [...prev, newEvent]);
    }

    closeModal();
  };

  const handleEventDrop = ({ event, start, end }: EventDropArg<TrainingEvent>) => {
    setEvents(prev => prev.map(ev => ev.id === event.id ? { ...ev, start, end } : ev));
  };

  const handleEventResize = ({ event, start, end }: EventResizeArg<TrainingEvent>) => {
    setEvents(prev => prev.map(ev => ev.id === event.id ? { ...ev, start, end } : ev));
  };

  const handleDeleteEvent = (id: string) => {
    setEvents(prev => prev.filter(e => e.id !== id));
    closeModal();
  };

  const filteredEvents = useMemo(() => {
    if (!selectedTrainer) return events;
    return onlyShowSelected ? events.filter(e => e.trainer === selectedTrainer) : events;
  }, [events, selectedTrainer, onlyShowSelected]);

  const eventStyleGetter = (event: TrainingEvent) => {
    const isHighlighted = selectedTrainer && event.trainer === selectedTrainer;
    const baseColor = event.color ?? "#60A5FA";
    return {
      style: {
        backgroundColor: isHighlighted ? baseColor : selectedTrainer ? "#E6E6E6" : baseColor,
        border: "1px solid rgba(0,0,0,0.08)",
        color: "#0f172a",
        paddingLeft: 6,
        borderRadius: 4,
        opacity: selectedTrainer && !isHighlighted ? 0.6 : 1,
      },
    };
  };

  const EventRenderer: React.FC<{ event: TrainingEvent }> = ({ event }) => (
    <div className="flex items-center justify-between">
      <div className="text-xs">
        <div className="font-medium truncate">{event.title}</div>
        <div className="text-[11px] text-gray-700 truncate">{event.trainer}</div>
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
            onChange={v => setSelectedTrainer(v ?? null)}
          />
          <Checkbox checked={onlyShowSelected} onChange={e => setOnlyShowSelected(e.target.checked)}>
            Только выбранный
          </Checkbox>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => openCreateModal({ start: new Date(), end: dayjs().add(1, "hour").toDate() })}
          >
            Создать
          </Button>
        </div>
      </div>

      <div style={{ height: "75vh" }}>
        <DnDCalendar
          views={['month', 'week', 'day']}
          defaultView="week"
          localizer={localizer}
          events={filteredEvents as RBCEvent[]}
          view={view}
          onView={setView}
          startAccessor="start"
          endAccessor="end"
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
            showMore: total => `+ ещё ${total}`
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
            render={({ field }) => <Select {...field} options={trainerOptions} placeholder="Тренер" />}
          />
          <Controller
            name="date"
            control={control}
            rules={{ required: true }}
            render={({ field }) => <DatePicker className="w-full" format="DD.MM.YYYY" {...field} value={field.value} />}
          />
          <div className="grid grid-cols-2 gap-3">
            <Controller
              name="timeStart"
              control={control}
              render={({ field }) => <TimePicker className="w-full" format="HH:mm" {...field} value={field.value} />}
            />
            <Controller
              name="timeEnd"
              control={control}
              render={({ field }) => <TimePicker className="w-full" format="HH:mm" {...field} value={field.value} />}
            />
          </div>
          <div className="flex justify-between items-center mt-4">
            <Space>
              <Button type="primary" htmlType="submit">{editingEvent ? "Сохранить" : "Создать"}</Button>
              {editingEvent && (
                <Popconfirm title="Удалить тренировку?" onConfirm={() => handleDeleteEvent(editingEvent.id)}>
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
