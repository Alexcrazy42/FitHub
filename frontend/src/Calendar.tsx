import '@/App.css'
import { Layout } from "antd";
import GymCalendar from "./components/Calendar/GymCalendar";
import VideoPlayerWithTimestamps from './components/Calendar/VideoPlayerWithTimestamps';

function App() {

  const videoUrl = 'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerJoyrides.mp4';
  const timestamps = [
    { id: '1', time: 0, title: 'Начало тренировки' },
    { id: '2', time: 4, title: 'Разминка' },
    { id: '3', time: 10, title: 'Основная часть' }
  ];

  return (
    <>
      <Layout style={{ minHeight: "100vh" }}>
        <Layout.Content style={{ padding: "50px" }}>
          <GymCalendar />
          <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
            <VideoPlayerWithTimestamps videoUrl={videoUrl} timestamps={timestamps} />
          </div>
        </Layout.Content>
      </Layout>
    </>
  )
}

export default App