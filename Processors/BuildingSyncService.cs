using Microsoft.EntityFrameworkCore;

namespace XakUjin2026
{
    // Сохраняет/обновляет здания, подъезды и устройства из ответа внешнего API.
    // Каждый подъезд получает по одному устройству на каждый тип из таблицы DeviceTypes.
    public static class BuildingSyncService
    {
        public static async Task SyncAsync(ApplicationDbContext context, BuildingListResponse response)
        {
            var buildings = response.data?.buildings;
            if (buildings == null || buildings.Count == 0)
                return;

            var deviceTypes = await context.DeviceTypes.ToListAsync();

            foreach (var item in buildings)
            {
                var ext = item.building;
                if (ext?.id == null)
                    continue;

                // Загружаем существующее здание вместе с подъездами и устройствами.
                var building = await context.Buildings
                    .Include(b => b.Entrances)
                        .ThenInclude(e => e.Devices)
                    .FirstOrDefaultAsync(b => b.Id == ext.id.Value);

                if (building == null)
                {
                    building = new BuildingEntity { Id = ext.id.Value };
                    context.Buildings.Add(building);
                }

                string? extAddress = ext.address?.GetFullAddress()?.Trim();

                // Обновляем поля здания (EF сам не пишет UPDATE, если ничего не изменилось).
                building.Title = ext.title;
                building.Address = extAddress;
                building.Alias = ext.alias;
                building.Floor = ext.floor;
                building.ApartmentCount = ext.apartmentCount;
                building.EntranceCount = ext.entranceCount;

                foreach (var extEntrance in item.entrances2 ?? new List<Entrance>())
                {
                    var entrance = building.Entrances
                        .FirstOrDefault(e => e.Number == extEntrance.number);

                    if (entrance == null)
                    {
                        entrance = new EntranceEntity { Number = extEntrance.number };
                        building.Entrances.Add(entrance);
                    }

                    entrance.FirstApartment = extEntrance.first_apartment;
                    entrance.LastApartment = extEntrance.last_apartment;

                    // Гарантируем наличие устройства каждого типа в подъезде.
                    foreach (var deviceType in deviceTypes)
                    {
                        var exists = entrance.Devices.Any(d => d.DeviceTypeId == deviceType.Id);
                        if (!exists)
                            entrance.Devices.Add(new DeviceEntity { DeviceTypeId = deviceType.Id });
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
