# Зона спортивного зала

Gym (Id, name, description, location и тд)

GymZone(Id, GymId, Name, MaxCapacity (???), IsAvailable)

ZoneBookings(Id, ZoneId, StartTime, EndTime, Status (активно/отменено))



# Групповые тренировки

Нужно сделать так, чтобы создание тренировок было на панели календаря, чтобы видеть, когда тренера могут и при этом создавались эти тренировки на месяц вперед

BaseGroupTrainingId(id, name, description, type, minuteDuration, complexity (1-3))
GroupTraining(id, baseId, time, trainer, participant[], gymId)
GroupTrainingParticipants(GroupTrainingId, UserId)


# Персональные тренировки

PersonalTraining (id, userId, trainerId, day, start, end)

# Видеотренировки

VideoTraining (id, name, description, complexity)