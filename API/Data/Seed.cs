using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(AppDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var members = System.Text.Json.JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

            if (members == null)
            {
                Console.WriteLine("No members found in the seed data.");
                return;
            }

            foreach (var member in members)
            {
                using var hmac = new HMACSHA512();
                var user = new AppUser
                {
                    Id = member.Id,
                    Email = member.Email,
                    ImageUrl = member.ImageUrl,
                    DisplayName = member.DisplayName,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                    PasswordSalt = hmac.Key,
                    Member = new Member
                    {
                        Id = member.Id,
                        Description = member.Description,
                        DateOfBirth = member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        DisplayName = member.DisplayName,
                        Created = member.Created,
                        LastActive = member.LastActive,
                        Gender = member.Gender,
                        City = member.City,
                        Country = member.Country,

                    }

                };

                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl,
                    MemberId = member.Id
                });

                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}