﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using StudentPortal.API.DataModels;
using System.Threading.Tasks;
using System;
using StudentPortal.API.DomainModels;

namespace StudentPortal.API.Repositories
{
    public class SqlStudentRepository : IStudentRepository
    {
        private readonly StudentContext context;

        public SqlStudentRepository(StudentContext context)
        {
            this.context = context;
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            return await context.Student.Include(nameof(Gender)).Include(nameof(Address)).ToListAsync();
        }
        
        public async Task<Student> GetStudentAsync(Guid studentId)
        {
            return await context.Student.Include(nameof(Gender)).Include(nameof(Address)).FirstOrDefaultAsync(x => x.Id == studentId);
        }

        public async Task<List<Gender>> GetGenderAsync()
        {
            return await context.Gender.ToListAsync();
        }

        public async Task<bool> Exists(Guid studentId)
        {
            return await context.Student.AnyAsync(x => x.Id == studentId);
        }

        public async Task<Student> UpdateStudent(Guid studentId, Student request)
        {
            var existingStudent = await GetStudentAsync(studentId);
            if (existingStudent != null)
            {
                existingStudent.FirstName = request.FirstName;
                existingStudent.LastName = request.LastName;
                existingStudent.DateofBirth = request.DateofBirth;
                existingStudent.Email = request.Email;
                existingStudent.PhoneNumber = request.PhoneNumber;
                existingStudent.GenderId = request.GenderId;
                existingStudent.Address.PhysicalAddress = request.Address.PhysicalAddress;
                existingStudent.Address.PostalAddress = request.Address.PostalAddress;

                await context.SaveChangesAsync();
                return existingStudent;
            }
            return null;
        }

        public async Task<Student> DeleteStudent(Guid studentId)
        {
            var student = await GetStudentAsync(studentId);
            if (student != null)
            {
                context.Student.Remove(student);
                await context.SaveChangesAsync();
                return student;
            }
            return null;
        }

        public async Task<Student> AddStudent(Student request)
        {
            var student = await context.Student.AddAsync(request);
            await context.SaveChangesAsync();
            return student.Entity;
        }

        public async Task<bool> UpdateProfileImage(Guid studentId, string profileImageUri)
        {
            var student = await GetStudentAsync(studentId);
            if (student != null)
            {
                student.ProfileImageUrl = profileImageUri;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
