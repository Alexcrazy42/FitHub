import React, { useState, useEffect } from "react";
import { Calendar, Badge, Select, Button, Divider, message } from "antd";
import type { Dayjs } from "dayjs";
import { fetchGymZones, fetchZoneBookings, createBooking } from "../../api/gymApi";
import BookingModal from "./BookingModal";
import ZoneStatusBadge from "./ZoneStatusBadge";
import { GymZone, ZoneBooking, CreateBookingDto } from "../../types/gymTypes";
import dayjs from "dayjs";

const GymCalendar: React.FC = () => {
  const [zones, setZones] = useState<GymZone[]>([]);
  const [selectedZone, setSelectedZone] = useState<number | null>(null);
  const [bookings, setBookings] = useState<ZoneBooking[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Загрузка зон и бронирований
  useEffect(() => {
    fetchGymZones().then(setZones);
  }, []);

  useEffect(() => {
    if (selectedZone) {
      fetchZoneBookings(selectedZone).then(setBookings);
    }
  }, [selectedZone]);

  const handleCreateBooking = async (booking: CreateBookingDto) => {
  try {
    const response = await createBooking(booking);
    if (response.success) {
      // Правильное добавление новой тренировки
      const newBooking: ZoneBooking = {
        ...booking,
        id: Date.now(),
        status: "active",
        zoneId: booking.zoneId // Добавляем zoneId
      };
      
      setBookings(prev => [...prev, newBooking]);
      message.success("Тренировка создана!");
    }
  } catch (error) {
    message.error("Ошибка при создании тренировки");
  }
};

  const dateCellRender = (date: Dayjs) => {
  const dayBookings = bookings.filter(b => {
    const bookingDate = new Date(b.startTime);
    return (
      bookingDate.getDate() === date.date() &&
      bookingDate.getMonth() === date.month() &&
      bookingDate.getFullYear() === date.year()
    );
  });

  return (
    <div className="events">
      {dayBookings.map(booking => (
        <div key={booking.id}>
          <Badge
            status={booking.status === "active" ? "success" : "error"}
            text={`${dayjs(booking.startTime).format("HH:mm")} - ${dayjs(booking.endTime).format("HH:mm")}`}
          />
        </div>
      ))}
    </div>
  );
};

  return (
    <div style={{ padding: "20px" }}>
      <Select<number>
        placeholder="Выберите зону"
        style={{ width: 200, marginBottom: 20 }}
        onChange={setSelectedZone}
        options={zones.map((zone) => ({
          value: zone.id,
          label: (
            <>
              <ZoneStatusBadge isAvailable={zone.isAvailable} /> {zone.name}
            </>
          ),
        }))}
      />

      <Button type="primary" onClick={() => setIsModalOpen(true)}>
        Создать тренировку
      </Button>

      <Divider />

      <Calendar dateCellRender={dateCellRender} />

      {selectedZone && (
        <BookingModal
          open={isModalOpen}
          onCancel={() => setIsModalOpen(false)}
          zoneId={selectedZone}
          onCreate={handleCreateBooking} // Передаём обработчик в модальное окно
        />
      )}
    </div>
  );
};

export default GymCalendar;