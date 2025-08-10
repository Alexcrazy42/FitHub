# Зона спортивного зала

Gym (Id, name, description, location и тд)


# Групповые тренировки

Нужно сделать так, чтобы создание тренировок было на панели календаря, чтобы видеть, когда тренера могут и при этом создавались эти тренировки на месяц вперед

BaseGroupTraining(id, name, description, type, minuteDuration, complexity (1-3))
GroupTraining(id, baseId, time, trainer, participant[], gymId)
GroupTrainingParticipants(GroupTrainingId, UserId)


# Персональные тренировки

PersonalTraining (id, userId, trainerId, day, start, end)

# Видеотренировки

VideoTraining (id, name, description, complexity)




чисто набросок: 

-- Тренер
CREATE TABLE Trainers (
    Id UUID PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);

-- Групповая тренировка (шаблон)
CREATE TABLE GroupWorkoutTemplates (
    Id UUID PRIMARY KEY,
    Name VARCHAR(100),
    DurationMinutes INT NOT NULL DEFAULT 60
);

-- Расписание групповых тренировок (конкретное время)
CREATE TABLE GroupWorkoutSchedules (
    Id UUID PRIMARY KEY,
    TemplateId UUID REFERENCES GroupWorkoutTemplates(Id),
    TrainerId UUID REFERENCES Trainers(Id),
    GymId UUID REFERENCES Gyms(Id),
    StartTime TIMESTAMPTZ NOT NULL,
    EndTime TIMESTAMPTZ NOT NULL
);

-- Персональная тренировка (конкретная сессия)
CREATE TABLE PersonalWorkouts (
    Id UUID PRIMARY KEY,
    TrainerId UUID REFERENCES Trainers(Id),
    ClientId UUID REFERENCES Users(Id),
    GymId UUID REFERENCES Gyms(Id),
    StartTime TIMESTAMPTZ NOT NULL,
    EndTime TIMESTAMPTZ NOT NULL
)



SELECT
    'personal' AS Type,
    Id,
    TrainerId,
    StartTime,
    EndTime,
    NULL AS TemplateId,
    NULL::UUID AS GymId -- или GymId, если есть
FROM PersonalWorkouts
WHERE TrainerId = '...'

UNION ALL

SELECT
    'group' AS Type,
    Id,
    TrainerId,
    StartTime,
    EndTime,
    TemplateId,
    GymId
FROM GroupWorkoutSchedules
WHERE TrainerId = '...'

ORDER BY StartTime;



-- Занятость за неделю
SELECT
    SUM(EXTRACT(EPOCH FROM (EndTime - StartTime)) / 3600) AS TotalHours
FROM (
    -- объединённый запрос как выше
) AS combined
WHERE StartTime >= '2025-04-01' AND EndTime < '2025-04-08';




public class TrainerScheduleItemDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } // "personal" или "group"
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string WorkoutName { get; set; } // название тренировки
    public Guid? GymId { get; set; }
}


public async Task<List<TrainerScheduleItemDto>> GetTrainerScheduleAsync(Guid trainerId, DateRange range)
{
    var personal = _context.PersonalWorkouts
        .Where(p => p.TrainerId == trainerId && p.StartTime >= range.Start && p.EndTime <= range.End)
        .Select(p => new TrainerScheduleItemDto
        {
            Id = p.Id,
            Type = "personal",
            StartTime = p.StartTime,
            EndTime = p.EndTime,
            WorkoutName = "Персональная тренировка",
            GymId = p.GymId
        });

    var group = _context.GroupWorkoutSchedules
        .Where(g => g.TrainerId == trainerId && g.StartTime >= range.Start && g.EndTime <= range.End)
        .Select(g => new TrainerScheduleItemDto
        {
            Id = g.Id,
            Type = "group",
            StartTime = g.StartTime,
            EndTime = g.EndTime,
            WorkoutName = g.Template.Name,
            GymId = g.GymId
        });

    return await personal
        .Union(group)
        .OrderBy(x => x.StartTime)
        .ToListAsync();
}


public async Task<bool> IsTrainerAvailable(Guid trainerId, DateTime start, DateTime end)
{
    var hasOverlap = await (
        from p in _context.PersonalWorkouts
        where p.TrainerId == trainerId
        where p.StartTime < end && p.EndTime > start
        select p
    ).AnyAsync();

    var hasGroupOverlap = await (
        from g in _context.GroupWorkoutSchedules
        where g.TrainerId == trainerId
        where g.StartTime < end && g.EndTime > start
        select g
    ).AnyAsync();

    return !(hasOverlap || hasGroupOverlap);
}




-- Можно считать по завершённым тренировкам
SELECT
    SUM(CASE WHEN Type = 'personal' THEN 1000 ELSE 500 END) AS TotalEarnings
FROM CombinedSchedule
WHERE TrainerId = '...' AND StartTime BETWEEN '...' AND '...'


