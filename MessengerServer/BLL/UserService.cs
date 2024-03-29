﻿using AutoMapper;
using MessengerServer.DAL.Entities;
using MessengerServer.DAL.Repositories;
using MessengerServer.DTOs.User;
using MessengerServer.Exceptions;
using MessengerServer.Util;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerServer.BLL
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly UserPictureRepository _userPictureRepository;
        private readonly IMapper _mapper;

        public UserService(
            UserRepository userRepository,
            UserPictureRepository userPictureRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userPictureRepository = userPictureRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Create method
        /// </summary>
        /// <param name="registerUserRequest"></param>
        /// <returns></returns>
        public async Task<User> RegisterAsync(User request)
        {
            User user = await _userRepository.FindByUserNameAsync(request.Username);
            if (user != null)
            {
                throw new NotUniqueException("User with that username is already exist");
            }

            user = _mapper.Map<User>(request);
            await _userRepository.CreateAsync(user);

            user = await _userRepository.FindByUserNameAsync(request.Username);
            return user;
        }

        /// <summary>
        /// Login with credentials
        /// </summary>
        /// <param name="loginUserRequest"></param>
        /// <returns></returns>
        public async Task<User> LoginAsync(UserAuthRequestDTO loginUserRequest)
        {
            User user = await _userRepository.FindWithUserCredentials(loginUserRequest.username, loginUserRequest.password);
            if (user is null)
            {
                throw new NotFoundException("User with that credentials wasn`t found");
            }

            return user;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        public Task<List<User>> GetUsers()
        {
            return _userRepository.GetAllAsync();
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetUserById(int id)
        {
            User user = await _userRepository.FindByIdAsync(id);
            if (user is null)
            {
                throw new NotFoundException("User was not found");
            }

            return user;
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUsername(string username)
        {
            User user = await _userRepository.FindByUserNameAsync(username);
            if (user is null)
            {
                throw new NotFoundException("User was not found");
            }

            return user;
        }

        /// <summary>
        /// Edit user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ParametersValidationException"></exception>
        public async Task EditAsync(IWebHostEnvironment webHostEnvironment, UserEditRequestDTO request)
        {
            User user = await _userRepository.FindByIdAsync(request.Id);
            if (user is null)
            {
                throw new NotFoundException("User with that id wasn`t found");
            }

            if ((await _userRepository.GetAllAsync()).Any(x => x.Username == request.Username && x.Id != request.Id))
            {
                throw new NotUniqueException("User with that username is already exist");
            }

            user.Username = request.Username;
            user.Password = request.Password;

            if (request.ImageBytes != null)
            {
                string newPath = await FileService.SaveFileInAvatarsFolder(webHostEnvironment, request.ImageBytes);

                if (user.UserPictureId.HasValue)
                {
                    UserPicture img = await _userPictureRepository.FindByIdAsync(user.UserPictureId.Value);

                    img.Path = newPath;
                    await _userPictureRepository.UpdateAsync(img);
                }
                else
                {
                    await _userPictureRepository.CreateAsync(new UserPicture { Path = newPath, UserId = user.Id });
                }
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task EditAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int userId)
        {
            User userToDelete = await _userRepository.FindByIdAsync(userId);
            if (userToDelete is null)
            {
                throw new NotFoundException("User not found");
            }

            await DeleteAsync(userToDelete);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(User user)
        {
            return _userRepository.DeleteAsync(user);
        }
    }
}
