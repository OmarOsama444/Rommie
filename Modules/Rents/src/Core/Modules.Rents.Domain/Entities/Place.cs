using NetTopologySuite.Geometries;

namespace Modules.Rents.Domain.Entities
{
    public class Place
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double? Rating { get; set; } = null;
        public int RateCount { get; set; } = 0;
        public int GovernorateId { get; set; }
        public Guid? UserId { get; set; }
        public Point Location { get; set; } = default!;
        public Governorate Governorate { get; set; } = default!;
        public User? User { get; set; }
        public static Place Create(
            User? user,
            string title,
            string description,
            int governorateId,
            double longitude,
            double latitude)
        {
            return new Place
            {
                UserId = user?.Id,
                User = user,
                Title = title,
                Description = description,
                GovernorateId = governorateId,
                Location = new Point(longitude, latitude) { SRID = 4326 }
            };
        }

    }


}