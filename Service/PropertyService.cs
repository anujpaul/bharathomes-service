using bharathome_api.DTOs;
using bharathome_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bharathome_api.Service
{
    public class PropertyService : IPropertyService
    {
        private readonly SqlDbContext _db;
        public PropertyService(SqlDbContext db)
        {
            _db = db;
        }

        public async Task<string> CreatePropertyAsync(CreatePropertyDto dto)
        {
            var validAgentIds = await _db.Agents
            .Where(a => dto.AgentId.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync();

            var property = new Property
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Price = dto.Price,
                Location = dto.Location,
                City = dto.City,
                Beds = dto.Beds,
                Baths = dto.Baths,
                Sqft = dto.Sqft,
                Type = dto.Type,
                IsFeatured = dto.IsFeatured,
                ExpresswayProximity = dto.ExpresswayProximity,
                // Normalize to "sell"/"rent" — DTO defaults to "sell", but
                // guard against "buy"/"sale" coming in from older clients.
                ListingIntent = string.Equals(dto.ListingIntent, "rent", StringComparison.OrdinalIgnoreCase)
                    ? "rent" : "sell",
                ListerId = dto.ListerId,
                Images = dto.Images.Select(url => new PropertyImage { Url = url }).ToList(),
                Amenities = dto.Amenities.Select(a => new PropertyAmenity { Name = a }).ToList(),
                PropertyAgents = validAgentIds
                    .Select(id => new PropertyAgent { AgentId = id })
                    .ToList(),
            };

            _db.Properties.Add(property);
            await _db.SaveChangesAsync();
            return property.Id;
        }

        public async Task<PropertyDetailsDto?> GetPropertyByIdAsync(string id)
        {
            var property = await _db.Properties
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PropertyDetailsDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Location = p.Location,
                City = p.City,
                Beds = p.Beds,
                Baths = p.Baths,
                Sqft = p.Sqft,
                Type = p.Type,
                IsFeatured = p.IsFeatured,
                ExpresswayProximity = p.ExpresswayProximity,
                IsReraRegistered = p.IsReraRegistered,
                ReraRegistrationNumber = p.ReraRegistrationNumber,
                VastuOrientation = p.VastuOrientation,
                CreatedAt = p.CreatedAt,

                Images = p.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => i.Url)
                .ToList(),

                Amenities = p.Amenities
                .Select(a => a.Name)
                .ToList(),

                Agents = p.PropertyAgents.Select(pa => new AgentDto
                {
                    Id = pa.Agent.Id,
                    Name = pa.Agent.UserProfile.Name,
                    Email = pa.Agent.UserProfile.Email,
                    Phone = pa.Agent.UserProfile.Phone,
                    UserPhoto = pa.Agent.UserProfile.UserPhoto,
                    Rating = pa.Agent.Rating,
                    ListingsCount = pa.Agent.ListingsCount,
                    Specialization = pa.Agent.Specialization
                }).ToList()
            })
        .FirstOrDefaultAsync();
            return property;
        }
    }
}
