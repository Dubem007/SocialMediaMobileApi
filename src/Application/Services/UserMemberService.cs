using Application.Contracts;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using Infrastructure.Contracts;
using Infrastructure.HttpHelper;
using Infrastructure.Utils.AWS;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace Application.Services
{
    public class UserMemberService : IUserMemberService
    {
        private readonly IRepositoryManager _repository;
        private readonly IAwsS3Client _awsS3Client;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IWebHelper _webHelper;
        private readonly IHttpClientHelper _clientHelper;
        private readonly ICacheServices _cache;
        public UserMemberService(IRepositoryManager repository, IAwsS3Client awsS3Client, UserManager<User> userManager, IMapper mapper, IWebHelper webHelper, IHttpClientHelper clientHelper, ICacheServices cache)
        {
            _repository = repository;
            _awsS3Client = awsS3Client;
            _userManager = userManager;
            _mapper = mapper;
            _webHelper = webHelper;
            _clientHelper = clientHelper;
            _cache = cache;
        }

        public async Task<SuccessResponse<UserMemberResponseDto>> CreateUserMember(UserMemberCreationInputDto input)
        {
            var referenceToken = await _repository.Token.FirstOrDefaultAsync(x => x.Value == input.ReferenceToken, false);
            if (referenceToken is null)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidExpiredToken);

            var member = await _repository.InitialMember.FirstOrDefaultAsync(x => x.Id == referenceToken.UserId, false);
            if (member is null)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidExpiredToken);

            if (!member.Email.Equals(input.Email, StringComparison.OrdinalIgnoreCase))
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidExpiredToken);

            if (member.RecognitionYear != input.RecognitionYear)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidExpiredToken);

            var isValid = CustomToken.IsTokenValid(referenceToken);
            if (!isValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            string profilePictureUrl = string.Empty;
            if (input.ProfileImage != null)
                profilePictureUrl = await _awsS3Client.UploadFileAsync(input.ProfileImage);

            var newUser = new User
            {
                UserName = input.Email.ToLower().Trim(),
                Email = input.Email.ToLower().Trim(),
                EmailConfirmed = true,
                FirstName = input.FirstName,
                LastName = input.LastName,
                ImageUrl = profilePictureUrl,
                IsActive = true,
                IsVerified = true,
                Status = EUserStatus.Active.ToString(),
            };
            newUser.PasswordHash = _userManager.PasswordHasher.HashPassword(newUser, input.Password);
            var result = await _userManager.CreateAsync(newUser, input.Password);
            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.BadRequest, result.Errors.FirstOrDefault().Description);
            var Country = input.Location.Split(',')[1];
            await _userManager.AddToRoleAsync(newUser, ERole.Regular.ToString());
            var locationdetails = _clientHelper.Location(input.Location);
            var userMember = _mapper.Map<UserMember>(input);
            userMember.UserId = newUser.Id;
            userMember.Longitude = locationdetails.Result.longitude;
            userMember.Latitude = locationdetails.Result.latitude;
            userMember.PrefferedName = input.PrefferedName == null ? newUser.FirstName : input.PrefferedName;
            userMember.Country = Country;

            await _repository.UserMember.CreateAsync(userMember);
            _repository.Token.Delete(referenceToken);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<UserMemberResponseDto>(input);
            response.ProfileImage = profilePictureUrl;
            response.Id = newUser.Id;
            response.UserMemberId = userMember.Id;
            response.CreatedAt = userMember.CreatedAt;

            return new SuccessResponse<UserMemberResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<UserMemberResponseDto>> UpdateUserMember(UserMemberUpdateInputDto input)
        {
            var userMember = await _repository.UserMember.FindByCondition(x => x.Id == input.UserMemberId, true)
                .Include(x => x.User)
                .FirstOrDefaultAsync();

            if (userMember is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            string profilePictureUrl = null;
            if (input.ProfileImage != null)
                profilePictureUrl = await _awsS3Client.UploadFileAsync(input.ProfileImage);
            var Country = input.Location.Split(',')[1];
            var locationdetails = _clientHelper.Location(input.Location);
            userMember.Longitude = locationdetails.Result.longitude;
            userMember.Latitude = locationdetails.Result.latitude;
            userMember.Country = Country;
            _mapper.Map(input, userMember);
            userMember.User.ImageUrl = profilePictureUrl ?? userMember.User.ImageUrl;

            _repository.UserMember.Update(userMember);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<UserMemberResponseDto>(userMember);

            return new SuccessResponse<UserMemberResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }


        public async Task<SuccessResponse<UserMemberResponseDto>> UpdateUserPhoto(UserMemberPhotoUpdateInputDto input)
        {
            var userMember = await _repository.UserMember.FindByCondition(x => x.Id == input.UserMemberId, true)
                .Include(x => x.User)
                .FirstOrDefaultAsync();

            if (userMember is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            string profilePictureUrl = null;
            if (input.ProfileEditImage != null)
                profilePictureUrl = await _awsS3Client.UploadFileAsync(input.ProfileEditImage);
            //_mapper.Map(input, userMember);
            userMember.User.ImageUrl = profilePictureUrl ?? userMember.User.ImageUrl;

            _repository.UserMember.Update(userMember);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<UserMemberResponseDto>(userMember);

            return new SuccessResponse<UserMemberResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<UserMemberResponseDto>> GetUserMemberById(Guid id)
        {
            var userId = _webHelper.User().UserId;
            var member = await _repository.UserMember.FindByCondition(x => x.UserId == id, false).Include(x => x.User).FirstOrDefaultAsync();
            if (member is null)
            {
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);
            }

            var userMember = await _repository.UserMember.FindByCondition(x => x.UserId == userId, false)
                .Include(x => x.User)
                .FirstOrDefaultAsync();

            var sentInvitation = _repository.Invitation.FindByCondition(x => x.RequesterId == userMember.Id || x.MemberId == userMember.Id, false);
            var sortedsentInvitations = sentInvitation.Where(x => x.MemberId == member.Id || x.RequesterId == member.Id).FirstOrDefault();

            var userMemberDto = _mapper.Map<UserMemberResponseDto>(member);
            if (sortedsentInvitations != null)
            {
                userMemberDto.ConnectionStatus = sortedsentInvitations.Status;
            }
            else
            {
                userMemberDto.ConnectionStatus = EInvitationStatus.None.ToString();
            }
            return new SuccessResponse<UserMemberResponseDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = userMemberDto
            };
        }

        public async Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUserMembers(SearchParameters parameters, string actionName, IUrlHelper urlHelper)
        {

            var userId = _webHelper.User().UserId;

            var member = await _repository.UserMember.FindByCondition(x => x.UserId == userId, false)
               .Include(x => x.User).FirstOrDefaultAsync();
            if (member == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);
            var invitations = _repository.Invitation.QueryAll(x => x.RequesterId == member.Id || x.MemberId == member.Id);

            var sortedmembers = await invitations.Select(x => new ConnectStatusDto { MemberId = x.MemberId, Status = x.Status }).ToListAsync();

            var userMemberQuery = _repository.UserMember.FindByCondition(x => x.UserId != member.UserId, false)
                .Include(x => x.User).Include(x => x.UserConnections) as IQueryable<UserMember>;

            var sortedsearchedmember = await SearchUsersMembers(userMemberQuery, parameters.Search, member);

            var userMemberDto = sortedsearchedmember.Select(x => new UserMemberResponseDto
            {
                Id = x.User.Id,
                UserMemberId = x.Id,
                UserId = x.UserId,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                PrefferedName = x.PrefferedName,
                Email = x.User.Email,
                ProfileImage = x.User.ImageUrl,
                DateOfBirth = x.DateOfBirth,
                RecognitionYear = x.RecognitionYear,
                ProfessionalField = x.ProfessionalField,
                Location = x.Location,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Bio = x.Bio,
                Country = x.Country,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsSubscribed = x.IsSubscribed,
                ConnectionStatus = GetConnectionStatus(x.Id, sortedmembers),
                UserConnections = x.UserConnections.Select(x => new UserConnectionDto
                {
                    MemberId = x.MemberId,
                    ConnectedMemberId = x.ConnectedMemberId
                })
            }).OrderBy(x => x.CreatedAt);
            var pagedUserMembersDto = await PagedList<UserMemberResponseDto>.Create(userMemberDto, parameters.PageNumber, parameters.PageSize, parameters.Sort);
            var dynamicParameters = PageUtility<UserMemberResponseDto>.GenerateResourceParameters(parameters, pagedUserMembersDto);
            var page = PageUtility<UserMemberResponseDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedUserMembersDto, urlHelper);

            return new PagedResponse<IEnumerable<UserMemberResponseDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedUserMembersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        private static string GetConnectionStatus(Guid memberId, List<ConnectStatusDto> invitations)
        {
            var status = invitations.Where(x => x.MemberId == memberId).Select(x => x.Status).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(status))
                return "None";

            return status;
        }

        public async Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUserMembersForUser(SearchUserMembersByUserParameters parameters, string value, string actionName, IUrlHelper urlHelper)
        {
            if (parameters.UserId == Guid.Empty)
            {
                return new PagedResponse<IEnumerable<UserMemberResponseDto>>
                {
                    Message = ResponseMessages.RetrievalSuccessResponse,
                    Data = null,
                    Meta = new Meta
                    {
                        Pagination = null
                    }
                };
            }

            var user = _repository.UserMember.FindByCondition(x => x.UserId == parameters.UserId, false)
               .Include(x => x.User).FirstOrDefault();

            var userMemberQuery = _repository.UserMember.FindByCondition(x => x.UserId != user.UserId, false)
                .Include(x => x.User) as IQueryable<UserMember>;

            var invitations = await _repository.Invitation.QueryAll(x => x.RequesterId == user.Id)
                .Select(x => new ConnectStatusDto { MemberId = x.MemberId, Status = x.Status })
                .ToListAsync();

            var query = userMemberQuery.Select(x => new UserMemberResponseDto
            {
                Id = x.User.Id,
                UserMemberId = x.Id,
                UserId = x.UserId,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                PrefferedName = x.PrefferedName,
                Email = x.User.Email,
                ProfileImage = x.User.ImageUrl,
                DateOfBirth = x.DateOfBirth,
                RecognitionYear = x.RecognitionYear,
                ProfessionalField = x.ProfessionalField,
                Location = x.Location,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Bio = x.Bio,
                Country = x.Country,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsSubscribed = x.IsSubscribed,
                ConnectionStatus = GetConnectionStatus(x.Id, invitations),
                UserConnections = x.UserConnections.Select(x => new UserConnectionDto
                {
                    MemberId = x.MemberId,
                    ConnectedMemberId = x.ConnectedMemberId
                })
            }).OrderBy(x => x.CreatedAt);

            ExpressionStarter<UserMemberResponseDto> predicate = GetQueryByUserDetails(user, value);
            var usermembers = query.Where(predicate);
            if (parameters.Search != null)
            {
                parameters.Search = parameters.Search.ToLowerInvariant();
                usermembers = usermembers.Where(x => x.FirstName.ToLower().StartsWith(parameters.Search) || x.LastName.ToLower().StartsWith(parameters.Search));


            }
            var pagedUserMembersDto = await PagedList<UserMemberResponseDto>.Create(usermembers, parameters.PageNumber, parameters.PageSize);
            var dynamicParameters = PageUtility<UserMemberResponseDto>.GenerateResourceParameters(parameters, pagedUserMembersDto);
            var page = PageUtility<UserMemberResponseDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedUserMembersDto, urlHelper);

            return new PagedResponse<IEnumerable<UserMemberResponseDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedUserMembersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };


        }

        public async Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUserMembersSuggestionsForUser(string actionName, IUrlHelper urlHelper)
        {
            var userId = _webHelper.User().UserId;
            string value = string.Empty;
            var user = _repository.UserMember.FindByCondition(x => x.UserId == userId, false)
               .Include(x => x.User).FirstOrDefault();

            var userMemberQuery = _repository.UserMember.FindByCondition(x => x.UserId != user.UserId && x.ProfessionalField == user.ProfessionalField, false)
                .Include(x => x.User) as IQueryable<UserMember>;

            var invitations = _repository.Invitation.QueryAll(x => x.RequesterId == user.Id || x.MemberId == user.Id);

            var sortedmembers = await invitations.Select(x => new ConnectStatusDto { MemberId = x.MemberId, Status = x.Status }).ToListAsync();

            var members = userMemberQuery.Select(x => x.Id).ToList();


            var query = userMemberQuery.Select(x => new UserMemberResponseDto
            {
                Id = x.User.Id,
                UserMemberId = x.Id,
                UserId = x.UserId,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                PrefferedName = x.PrefferedName,
                Email = x.User.Email,
                ProfileImage = x.User.ImageUrl,
                DateOfBirth = x.DateOfBirth,
                RecognitionYear = x.RecognitionYear,
                ProfessionalField = x.ProfessionalField,
                Location = x.Location,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Bio = x.Bio,
                Country = x.Country,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsSubscribed = x.IsSubscribed,
                ConnectionStatus = GetConnectionStatus(x.Id, sortedmembers),
                UserConnections = x.UserConnections.Select(x => new UserConnectionDto
                {
                    MemberId = x.MemberId,
                    ConnectedMemberId = x.ConnectedMemberId
                })
            }).OrderBy(x => x.CreatedAt);

            ExpressionStarter<UserMemberResponseDto> predicate = GetQueryByUserDetails(user, value);
            var usermembers = query.Where(predicate);

            var suggestedmembers = usermembers.Where(x => x.ProfessionalField == user.ProfessionalField || x.Location == user.Location).OrderBy(x => x.CreatedAt);
            return new PagedResponse<IEnumerable<UserMemberResponseDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = suggestedmembers,

            };


        }

        private async Task<IQueryable<UserMember>> SearchUsersMembers(IQueryable<UserMember> users, string parameter, UserMember member)
        {


            if (!string.IsNullOrWhiteSpace(parameter))
            {
                var search = parameter.Trim().ToLower();
                var splitSearch = search.Split(' ');

                if (splitSearch.Length > 1)
                {
                    users = users.Where(x =>
                        (x.User.FirstName.ToLower().Contains(splitSearch[0].Trim()) || x.User.LastName.ToLower().Contains(splitSearch[1].Trim())) ||
                        (x.User.LastName.ToLower().Contains(splitSearch[0].Trim()) || x.User.FirstName.ToLower().Contains(splitSearch[1].Trim())) ||

                        (x.PrefferedName.ToLower().Contains(splitSearch[0].Trim()) || x.User.Email.ToLower().Contains(splitSearch[1].Trim())) ||
                        (x.User.Email.ToLower().Contains(splitSearch[0].Trim()) || x.PrefferedName.ToLower().Contains(splitSearch[1].Trim())) ||

                        (x.RecognitionYear.ToLower().Contains(splitSearch[0].Trim()) || x.ProfessionalField.ToLower().Contains(splitSearch[1].Trim())) ||
                        (x.ProfessionalField.ToLower().Contains(splitSearch[0].Trim()) || x.RecognitionYear.ToLower().Contains(splitSearch[1].Trim())) ||

                        (x.Location.ToLower().Contains(splitSearch[0].Trim()) || x.Location.ToLower().Contains(splitSearch[1].Trim())) ||
                        (x.Country.ToLower().Contains(splitSearch[0].Trim()) || x.Country.ToLower().Contains(splitSearch[0].Trim())));
                }
                else
                {
                    users = users.Where(x => x.User.FirstName.ToLower().Trim().Contains(search) ||
                        x.User.LastName.ToLower().Trim().Contains(search) || x.PrefferedName.ToLower().Trim().Contains(search) || x.User.Email.ToLower().Trim().Contains(search) ||
                        x.RecognitionYear.ToLower().Trim().Contains(search) || x.Location.ToLower().Trim().Contains(search) || x.Country.ToLower().Trim().Contains(search) ||
                        x.ProfessionalField.ToLower().Trim().Contains(search) || x.Bio.ToLower().Trim().Contains(search));
                }


            }

            users = users
              .OrderByDescending(x => x.CreatedAt)
              .ThenBy(x => x.UpdatedAt);



            return users;

        }


        private static ExpressionStarter<UserMember> GetUsersQueryPredicateBuilder(UserMember user, UserMemberParameters parameters)
        {

            var predicate = PredicateBuilder.New<UserMember>(true);

            if (!string.IsNullOrWhiteSpace(parameters.ProfessionalField))
            {
                var professionalField = parameters.ProfessionalField.ToLower();
                if (user.IsSubscribed)
                {

                    predicate = predicate.Or(p => p.ProfessionalField.ToLower().Contains(professionalField));
                }
                else
                {
                    var recognitionyear = user.RecognitionYear.ToString();
                    predicate = predicate.Or(p => p.ProfessionalField.Contains(professionalField) && p.RecognitionYear.Contains(recognitionyear));
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.RecognitionYear))
            {
                var recognitionYear = parameters.RecognitionYear.ToLower();
                predicate = predicate.Or(p => p.RecognitionYear.ToLower().Contains(recognitionYear));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Location))
            {
                var location = parameters.Location.ToLower();

                if (user.IsSubscribed)
                {

                    predicate = predicate.Or(p => p.Location.ToLower().Contains(location));
                }
                else
                {
                    var recognitionyear = user.RecognitionYear.ToLower().ToString();
                    predicate = predicate.Or(p => p.Location.ToLower().Contains(location) && p.RecognitionYear.ToLower().Contains(recognitionyear));
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.Email))
            {
                var email = parameters.Email.ToLower().ToString();

                if (user.IsSubscribed)
                {

                    predicate = predicate.Or(p => p.User.Email.ToLower().Contains(email));
                }
                else
                {
                    var recognitionyear = user.RecognitionYear.ToLower();
                    predicate = predicate.Or(p => p.User.Email.ToLower().Contains(email) && p.RecognitionYear.ToLower().Contains(recognitionyear));
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.FirstName))
            {
                var firstName = parameters.FirstName.ToLower();

                if (user.IsSubscribed)
                {

                    predicate = predicate.Or(p => p.User.FirstName.ToLower().Contains(firstName));
                }
                else
                {
                    var recognitionyear = user.RecognitionYear.ToLower().ToString();
                    predicate = predicate.Or(p => p.User.FirstName.ToLower().Contains(firstName) && p.RecognitionYear.ToLower().Contains(recognitionyear));
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.LastName))
            {
                var lastName = parameters.LastName.ToLower();

                if (user.IsSubscribed)
                {

                    predicate = predicate.Or(p => p.User.LastName.ToLower().Contains(lastName));
                }
                else
                {
                    var recognitionyear = user.RecognitionYear.ToLower();
                    predicate = predicate.Or(p => p.User.LastName.ToLower().Contains(lastName) && p.RecognitionYear.ToLower().Contains(recognitionyear));
                }
            }

            return predicate;
        }

        private static ExpressionStarter<UserMemberResponseDto> GetQueryByUserDetails(UserMember user, string parameter)
        {
            var predicate = PredicateBuilder.New<UserMemberResponseDto>(true);



            if (!string.IsNullOrWhiteSpace(parameter) && parameter == "ProfessionalField" && user != null)
            {
                var professionalField = user.ProfessionalField.ToString();

                predicate = predicate.Or(p => p.ProfessionalField.ToLower().Contains(professionalField.ToLower()));

            }
            if (!string.IsNullOrWhiteSpace(parameter) && parameter == "Location" && user != null)
            {
                var Location = user.Location.ToString();

                predicate = predicate.Or(p => p.Location.ToLower().Contains(Location.ToLower()));

            }

            if (!string.IsNullOrWhiteSpace(parameter) && parameter == "RecognitionYear" && user != null)
            {
                var RecognitionYear = user.RecognitionYear.ToString();
                predicate = predicate.Or(p => p.RecognitionYear.ToLower().Contains(RecognitionYear.ToLower()));
            }

            return predicate;
        }

        public async Task<SuccessResponse<GroupUserMembersDto>> MembersGroupByIdAsync(Guid Id)
        {
            var response = new SuccessResponse<GroupUserMembersDto>();
            var userId = _webHelper.User().UserId;
            var user = await _repository.UserMember.FindByCondition(x => x.UserId == userId, false)
               .Include(x => x.User).FirstOrDefaultAsync();

            var memberId = user.Id;
            var result = _repository.UserConnection.FindByCondition(x => x.MemberId == memberId || x.ConnectedMemberId == memberId, false);
            var usermemberIds = result.Select(x => x.MemberId);
            var connectedmemberIds = result.Select(x => x.ConnectedMemberId);
            var allusermembers = _repository.UserMember.QueryAll(x => usermemberIds.Any(y => y == x.Id) || connectedmemberIds.Any(z => z == x.Id))
              .Include(x => x.User) as IQueryable<UserMember>;

            var memberspergroup = allusermembers.GroupBy(x => x.ProfessionalField).Select(x => new MemberGroupDto { Group = x.Key, Members = x.ToList() }).ToList();
            var groups = _repository.MemberGroup.QueryAll();
            memberspergroup = memberspergroup
              .Join(groups,
                    p => p.Group,
                    e => e.Group,
                    (p, e) => new MemberGroupDto
                    {
                        Id = e.GroupId,
                        Group = e.Group,
                        Description = e.Decription,
                        Members = p.Members,
                        Counts = p.Members.Count().ToString(),
                    }
                    ).ToList();

            var groupmembers = memberspergroup.FirstOrDefault(x => x.Id == Id);

            if (groupmembers == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.GroupNotExist);

            var membercountpergroup = _mapper.Map<GroupUserMembersDto>(groupmembers);
            var groupCount = await groups.CountAsync();

            response.Message = ResponseMessages.DataRetrievedSuccessfully;
            response.Data = membercountpergroup;

            return response;

        }

        public async Task<SuccessResponse<IEnumerable<GroupUserMembersDto>>> MembersByGroupCountAsync()
        {
            var response = new SuccessResponse<IEnumerable<GroupUserMembersDto>>();

            var userId = _webHelper.User().UserId;
            var user = await _repository.UserMember.FindByCondition(x => x.UserId == userId, false)
               .Include(x => x.User).FirstOrDefaultAsync();

            var memberId = user.Id;
            var result = _repository.UserConnection.FindByCondition(x => x.MemberId == memberId || x.ConnectedMemberId == memberId, false);
            var usermemberIds = result.Select(x => x.MemberId);
            var connectedmemberIds = result.Select(x => x.ConnectedMemberId);
            var allusermembers = _repository.UserMember.QueryAll(x => usermemberIds.Any(y => y == x.Id) || connectedmemberIds.Any(z => z == x.Id))
              .Include(x => x.User) as IQueryable<UserMember>;

            var memberspergroup = allusermembers.GroupBy(x => x.ProfessionalField).Select(x => new MemberGroupDto { Group = x.Key, Members = x.ToList() }).ToList();
            var groups = _repository.MemberGroup.QueryAll();
            memberspergroup = memberspergroup
              .Join(groups,
                    p => p.Group,
                    e => e.Group,
                    (p, e) => new MemberGroupDto
                    {
                        Id = e.GroupId,
                        Group = e.Group,
                        Members = p.Members,
                        Description = e.Decription,
                        Counts = p.Members.Count().ToString(),
                    }
                    ).ToList();
            var groupCount = await groups.CountAsync();
            var membercountpergroup = _mapper.Map<List<GroupUserMembersDto>>(memberspergroup);

            response.Message = ResponseMessages.DataRetrievedSuccessfully;
            response.Data = membercountpergroup;

            return response;

        }

        public async Task<SuccessResponse<IEnumerable<ListOfProfessionsDto>>> GetAllProfessionalFields()
        {

            var response = new SuccessResponse<IEnumerable<ListOfProfessionsDto>>();

            var result = await _repository.ProfessionalField.GetAllAsync();
            var listofprofessions = result.Select(x => new ListOfProfessionsDto
            {
                Id = x.Id,
                Profession = x.Profession

            }).ToList();

            response.Message = ResponseMessages.DataRetrievedSuccessfully;
            response.Data = listofprofessions;

            return response;

        }

        public async Task<PagedResponse<IEnumerable<MembersByLocationDto>>> ConnectionsNearby(ResourceParameters parameters, string actionName, IUrlHelper urlHelper)
        {
            var userId = _webHelper.User().UserId;

            var member = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == userId, false);
            var allMembers = _repository.UserMember.QueryAll();

            var membersByLattitude = allMembers.Where(x => x.Latitude >= member.Latitude - 0.45 && x.Latitude <= member.Latitude + 0.45);
            var membersByLongitude = membersByLattitude.Where(x => x.Longitude >= member.Longitude - 0.45 && x.Longitude <= member.Longitude + 0.45 && x.Id != member.Id);

            var connectionsNearby = membersByLongitude.Where(x => x.UserConnections.Any(y => y.ConnectedMemberId == member.Id || y.MemberId == member.Id));

            var members = connectionsNearby.Select(x => new MembersByLocationDto
            {
                MemberId = x.Id,
                UserId = x.UserId,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Location = x.Location,
                ImageUrl = x.User.ImageUrl,
                ProfessionalField = x.ProfessionalField,
                Country = x.Country,
            });

            var pagedMembersDto = await PagedList<MembersByLocationDto>.Create(members, parameters.PageNumber, parameters.PageSize);
            var dynamicParameters = PageUtility<MembersByLocationDto>.GenerateResourceParameters(parameters, pagedMembersDto);
            var page = PageUtility<MembersByLocationDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedMembersDto, urlHelper);

            return new PagedResponse<IEnumerable<MembersByLocationDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedMembersDto,
                Meta = new Meta
                {
                    Pagination = page
                },
            };
        }

        public async Task<Predictions> LocationPredictions(MembersByLocationParameters parameters, string actionName, IUrlHelper urlHelper)
        {
            var response = await _clientHelper.LocationPredictions(parameters.Location);

            return response;
        }
        public async Task<LocationDto> Location(MembersByLocationParameters parameters, string actionName, IUrlHelper urlHelper)
        {
            var response = await _clientHelper.Location(parameters.Location);

            return response;
        }

        public async Task<LocationDto> LongitudeAndLatitlude(LocationByPlaceIdParameters parameters, string actionName, IUrlHelper urlHelper)
        {
            var response = await _clientHelper.LongitudeAndLatitlude(parameters.PlaceId);

            return response;
        }

        public async Task<PagedResponse<IEnumerable<BreakdownDto>>> GetAllByTheLocationAsync(string actionName, IUrlHelper urlHelper)
        {
            try
            {
                // Get Data from Redis database
                var cacheData = await _cache.GetData<IEnumerable<BreakdownDto>>("breakdowns");

                if (cacheData != null)
                {
                    return new PagedResponse<IEnumerable<BreakdownDto>>
                    {
                        Message = ResponseMessages.RetrievalSuccessResponse,
                        Data = cacheData,

                    };
                }
                var result = _repository.UserMember.QueryAll() as IQueryable<UserMember>;
                var res = result.OrderBy(x => x.Location);
                var breakdowns = result.GroupBy(x => x.Location).Select(x => new BreakdownDto { Location = x.Key, Counts = x.Count().ToString(), latitude = x.Select(x => x.Latitude).First(), longitude = x.Select(x => x.Longitude).First() });

                // Set Data to Redis database
                var expirationTime = DateTimeOffset.Now.AddMinutes(30.0);
                var bar = await _cache.SetData<IQueryable<BreakdownDto>>("breakdowns", breakdowns, expirationTime);

                return new PagedResponse<IEnumerable<BreakdownDto>>
                {
                    Message = ResponseMessages.RetrievalSuccessResponse,
                    Data = breakdowns,

                };
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<PagedResponse<IEnumerable<UserMemberResponseDto>>> GetUsersPerLocationAsync(SearchUserMembersBylocationParameters parmmeters, string search, string actionName, IUrlHelper urlHelper)
        {
            try
            {
                // Get Data from Redis database
                var cacheData = await _cache.GetData<IEnumerable<UserMemberResponseDto>>("bylocations");

                if (cacheData != null)
                {
                    if (search != null)
                    {
                        search = search.Trim().ToLowerInvariant();
                        cacheData = cacheData.Where(x => x.FirstName.Trim().ToLower().StartsWith(search) || x.LastName.Trim().ToLower().StartsWith(search));
                    }
                    return new PagedResponse<IEnumerable<UserMemberResponseDto>>
                    {
                        Message = ResponseMessages.RetrievalSuccessResponse,
                        Data = cacheData,

                    };
                }
                //var searchparamter = parmmeters.Location.ToString().Trim().ToLower();
                string value = string.Empty;
                var userId = _webHelper.User().UserId;
                var user = _repository.UserMember.FindByCondition(x => x.UserId == userId, false)
                   .Include(x => x.User).FirstOrDefault();
                var userMemberQuery = _repository.UserMember.FindByCondition(x => x.UserId != user.UserId, false)
                 .Include(x => x.User) as IQueryable<UserMember>;
                var membersinlocation = userMemberQuery.Where(x => x.Longitude.ToString() == parmmeters.Longitude.ToString() && x.Latitude.ToString() == parmmeters.Latitude.ToString()) as IQueryable<UserMember>;
                var invitations = _repository.Invitation.QueryAll(x => x.RequesterId == user.Id || x.MemberId == user.Id);
                if (search != null)
                {
                    search = search.Trim().ToLowerInvariant();
                    membersinlocation = membersinlocation.Where(x => x.User.FirstName.Trim().ToLower().StartsWith(search) || x.User.LastName.Trim().ToLower().StartsWith(search));
                }
                var sortedmembers = await invitations.Select(x => new ConnectStatusDto { MemberId = x.MemberId, Status = x.Status }).ToListAsync();

                var query = membersinlocation.Select(x => new UserMemberResponseDto
                {
                    Id = x.User.Id,
                    UserMemberId = x.Id,
                    UserId = x.UserId,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    PrefferedName = x.PrefferedName,
                    Email = x.User.Email,
                    ProfileImage = x.User.ImageUrl,
                    DateOfBirth = x.DateOfBirth,
                    RecognitionYear = x.RecognitionYear,
                    ProfessionalField = x.ProfessionalField,
                    Location = x.Location,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Bio = x.Bio,
                    Country = x.Country,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    IsSubscribed = x.IsSubscribed,
                    ConnectionStatus = GetConnectionStatus(x.Id, sortedmembers),
                    UserConnections = x.UserConnections.Select(x => new UserConnectionDto
                    {
                        MemberId = x.MemberId,
                        ConnectedMemberId = x.ConnectedMemberId
                    })
                }).OrderBy(x => x.CreatedAt);

                ExpressionStarter<UserMemberResponseDto> predicate = GetQueryByUserDetails(user, value);
                var usermembers = query.Where(predicate);
                // Set Data to Redis database
                var expirationTime = DateTimeOffset.Now.AddMinutes(30.0);
                var bar = await _cache.SetData<IQueryable<UserMemberResponseDto>>("bylocations", usermembers, expirationTime);
                return new PagedResponse<IEnumerable<UserMemberResponseDto>>
                {
                    Message = ResponseMessages.RetrievalSuccessResponse,
                    Data = usermembers,

                };

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<SuccessResponse<DeviceTokenDto>> CreateDeviceToken(DeviceTokenCreateDto model)
        {
            var devicetoken = _mapper.Map<DeviceToken>(model);
            devicetoken.CreatedById = devicetoken.UserId;
            await _repository.DeviceToken.CreateAsync(devicetoken);

            UserActivity userActivity = AuditLog.UserActivity(devicetoken, model.UserId, nameof(devicetoken), $"Device Token added", devicetoken.Id);
            await _repository.UserActivity.CreateAsync(userActivity);
            await _repository.SaveChangesAsync();
            var response = _mapper.Map<DeviceTokenDto>(devicetoken);
            return new SuccessResponse<DeviceTokenDto>
            {
                Data = response,
                Message = "Data created successfully",
                Success = true
            };

        }
    }
}
