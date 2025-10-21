import React, { useEffect, useRef } from "react";
import maplibregl from "maplibre-gl";
import "maplibre-gl/dist/maplibre-gl.css";

export const MapCard: React.FC = () => {
  const mapContainer = useRef<HTMLDivElement | null>(null);
  const mapInstance = useRef<maplibregl.Map | null>(null);

  
  const MAPTILER_KEY = "";

  const places = [
    { id: 1, name: "Офис", lat: 55.751244, lon: 37.618423 },
    { id: 2, name: "Магазин", lat: 55.7601, lon: 37.6205 },
    { id: 3, name: "Склад", lat: 55.745, lon: 37.605 },
  ];

  useEffect(() => {
    if (!mapContainer.current) return;

    // Инициализация карты
    const map = new maplibregl.Map({
      container: mapContainer.current,
      style: `https://api.maptiler.com/maps/streets/style.json?key=${MAPTILER_KEY}`,
      center: [37.618423, 55.751244],
      zoom: 11,
    });

    // Добавляем стандартные контролы
    map.addControl(new maplibregl.NavigationControl(), "top-right");

    // Добавляем маркеры
    places.forEach((place) => {
      new maplibregl.Marker({ color: "#2563EB" })
        .setLngLat([place.lon, place.lat])
        .setPopup(
          new maplibregl.Popup({ offset: 25 }).setHTML(`
            <div class="text-sm font-semibold">${place.name}</div>
          `)
        )
        .addTo(map);
    });

    mapInstance.current = map;

    // Очистка при размонтировании
    return () => map.remove();
  }, []);

  return (
    <div className="w-full h-[500px] rounded-2xl overflow-hidden shadow-lg border border-gray-200 bg-white relative">
      <div ref={mapContainer} className="w-full h-full" />
    </div>
  );
};
