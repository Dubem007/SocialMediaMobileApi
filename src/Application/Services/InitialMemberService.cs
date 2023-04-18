using Application.Contracts;
using Application.Helpers;
using AutoMapper;
using CsvHelper;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Contracts;
using Shared;
using Shared.DataTransferObjects;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace Application.Services
{
    public class InitialMemberService : IInitialMemberService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        public InitialMemberService(IMapper mapper, IRepositoryManager repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<SuccessResponse<InitialMemberResponseDto>> GetInitialMemberByEmail(InitialMemberInputDto input)
        {
            var initialMember = await _repository.InitialMember.FirstOrDefaultAsync(x => x.Email.ToLower() == input.Email.ToLower(), false);
            if (initialMember == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InitialMemberNotFound);

            var isEmailExist = await _repository.User.ExistsAsync(x => x.Email == input.Email);

            var response = _mapper.Map<InitialMemberResponseDto>(initialMember);
            response.IsEmailExist = isEmailExist;

            return new SuccessResponse<InitialMemberResponseDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.ToLower().Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address.ToLower() == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public async Task<SuccessResponse<UploadInitialMemberResponseDto>> UploadBulkInitialMembers(UploadInitialMemberInputDto input)
        {
            List<InitialMemberResponseDto> initialMemberResponse = new();
            Dictionary<string, string> errors = new();
            List<UploadInitialMemeberDto> members = new();
            HashSet<int> invalidRecords = new();

            using var reader = new StreamReader(input.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            try
            {
                members = csv.GetRecords<UploadInitialMemeberDto>().ToList();
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid header(s) name");
            }

            for (int i = 0; i < members.Count; i++)
            {
                if (!IsValidEmail(members[i].Email))
                {
                    errors.Add($"Email[{i}]", "Invalid email");
                    invalidRecords.Add(i);
                }
                if (string.IsNullOrWhiteSpace(members[i].RecognitionYear))
                {
                    errors.Add($"YearOfRecognition[{i}]", "Year of recognition is required");
                    invalidRecords.Add(i);
                }
            }

            var validRecords = members.Where((member, index) => !invalidRecords.Contains(index)).ToList();
            var valideRecordEmails = validRecords.Select(x => x.Email.ToLower().Trim()).ToList();
            var existingMembers = _repository.InitialMember.FindByCondition(x => valideRecordEmails.Contains(x.Email.ToLower().Trim()), false).Select(x => x.Email.ToLower().Trim()).ToList();

            for (int i = 0; i < existingMembers.Count; i++)
            {
                var index = members.FindIndex(x => x.Email == existingMembers[i]);

                if (index > -1)
                {
                    errors.Add($"Email[{i}]", $"Email '{existingMembers[i]}' already exist");
                    invalidRecords.Add(i);
                }
            }

            var initialMemberDto = members.Where((member, index) => !invalidRecords.Contains(index)).ToList();
            var initialMembers = _mapper.Map<List<InitialMember>>(initialMemberDto);

            await _repository.InitialMember.AddRangeAsync(initialMembers);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<List<InitialMemberResponseDto>>(initialMembers);

            return new SuccessResponse<UploadInitialMemberResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = new UploadInitialMemberResponseDto
                {
                    Errors = errors,
                    InitialMembers = response,
                    Message = $"{initialMembers.Count} record(s) saved."
                }
            };
        }

        public async Task<SuccessResponse<UploadProfessionalListResponseDto>> UploadBulkProfessionalList(UploadProfessionalListInputDto input)
        {
            List<ProfessionalListResponseDto> professionsResponse = new();
            Dictionary<string, string> errors = new();
            List<UploadProfessionalListDto> professions = new();
            HashSet<int> invalidRecords = new();

            using var reader = new StreamReader(input.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            try
            {
                professions = csv.GetRecords<UploadProfessionalListDto>().ToList();
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid header(s) name");
            }

            for (int i = 0; i < professions.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(professions[i].Profession))
                {
                    errors.Add($"professions[{i}]", "profession is required");
                    invalidRecords.Add(i);
                }
            }

            var validRecords = professions.Where((professions, index) => !invalidRecords.Contains(index)).ToList();
            var existingRecord = _repository.ProfessionalField.FindByCondition(x => x.Profession.Contains(x.Profession), false).Select(x => x.Profession).ToList();

            for (int i = 0; i < existingRecord.Count; i++)
            {
                var index = professions.FindIndex(x => x.Profession == professions[i].Profession);

                if (index > -1)
                {
                    errors.Add($"professions[{i}]", $"professions '{professions[i]}' already exist");
                    invalidRecords.Add(i);
                }
            }

            var professionalListDto = professions.Where((professions, index) => !invalidRecords.Contains(index)).ToList();
            var professionaLists = _mapper.Map<List<Professions>>(professionalListDto);

            await _repository.ProfessionalField.AddRangeAsync(professionaLists);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<List<ProfessionalListResponseDto>>(professionaLists);

            return new SuccessResponse<UploadProfessionalListResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = new UploadProfessionalListResponseDto
                {
                    Errors = errors,
                    Professions = response,
                    Message = $"{professionaLists.Count} record(s) saved."
                }
            };
        }


        public async Task<SuccessResponse<UploadSubscriptionResponseDto>> UploadSubscriptions(UploadSubscriptionInputDto input)
        {
            List<UploadSubscriptionResponseDto> subscriptionResponse = new();
            Dictionary<string, string> errors = new();
            List<UploadSubscriptionDto> subscriptions = new();
            HashSet<int> invalidRecords = new();

            using var reader = new StreamReader(input.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            try
            {
                subscriptions = csv.GetRecords<UploadSubscriptionDto>().ToList();
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid header(s) name");
            }

            for (int i = 0; i < subscriptions.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(subscriptions[i].SubscriptionPlan))
                {
                    errors.Add($"SubscriptionPlan[{i}]", "SubscriptionPlan is required");
                    invalidRecords.Add(i);
                }
            }

            var validRecords = subscriptions.Where((subscriptions, index) => !invalidRecords.Contains(index)).ToList();
            var existingRecord = _repository.Subscriptions.FindByCondition(x => x.SubscriptionPlan.Contains(x.SubscriptionPlan), false).Select(x => x.SubscriptionPlan).ToList();

            for (int i = 0; i < existingRecord.Count; i++)
            {
                var index = subscriptions.FindIndex(x => x.SubscriptionPlan == subscriptions[i].SubscriptionPlan);

                if (index > -1)
                {
                    errors.Add($"subscriptions[{i}]", $"subscriptions '{subscriptions[i]}' already exist");
                    invalidRecords.Add(i);
                }
            }

            var subscriptionsDto = subscriptions.Where((subscriptions, index) => !invalidRecords.Contains(index)).ToList();
            var subscriptionsList = _mapper.Map<List<Subscription>>(subscriptionsDto);

            await _repository.Subscriptions.AddRangeAsync(subscriptionsList);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<List<SubscriptionResponseDto>>(subscriptionsList);

            return new SuccessResponse<UploadSubscriptionResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = new UploadSubscriptionResponseDto
                {
                    Errors = errors,
                    SubscriptionPlan = response,
                    Message = $"{subscriptionsList.Count} record(s) saved."
                }
            };
        }

        public async Task<SuccessResponse<UploadPremiumPlansResponseDto>> UploadPremiumPlans(UploadPremiumPlansInputDto input)
        {
            List<UploadPremiumPlansResponseDto> premiumplansResponse = new();
            Dictionary<string, string> errors = new();
            List<UploadPremiumPlansDto> premiumplans = new();
            HashSet<int> invalidRecords = new();

            using var reader = new StreamReader(input.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            try
            {
                premiumplans = csv.GetRecords<UploadPremiumPlansDto>().ToList();
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid header(s) name");
            }

            for (int i = 0; i < premiumplans.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(premiumplans[i].Duration))
                {
                    errors.Add($"professions[{i}]", "profession is required");
                    invalidRecords.Add(i);
                }
            }

            var validRecords = premiumplans.Where((premiumplans, index) => !invalidRecords.Contains(index)).ToList();
            var valideRecordDuration = validRecords.Select(x => x.Duration).ToList();
            var existingRecord = _repository.PremiumPlans.FindByCondition(x => valideRecordDuration.Contains(x.Duration), false).Select(x => x.Duration).ToList();

            for (int i = 0; i < existingRecord.Count; i++)
            {
                var index = premiumplans.FindIndex(x => x.Duration == premiumplans[i].Duration);

                if (index > -1)
                {
                    errors.Add($"premiumplans[{i}]", $"premiumplans '{premiumplans[i]}' already exist");
                    invalidRecords.Add(i);
                }
            }

            var premiumplansDto = premiumplans.Where((premiumplans, index) => !invalidRecords.Contains(index)).ToList();
            var premiumplansLists = _mapper.Map<List<PremiumPlan>>(premiumplansDto);

            await _repository.PremiumPlans.AddRangeAsync(premiumplansLists);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<List<PremiumPlansResponseDto>>(premiumplansLists);

            return new SuccessResponse<UploadPremiumPlansResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = new UploadPremiumPlansResponseDto
                {
                    Errors = errors,
                    PremiumPlans = response,
                    Message = $"{premiumplansLists.Count} record(s) saved."
                }
            };
        }

        public async Task<SuccessResponse<UploadMemberGroupResponseDto>> UploadMemberGroup(UploadMemberGroupInputDto input)
        {
            List<UploadMemberGroupResponseDto> memberGroupResponse = new();
            Dictionary<string, string> errors = new();
            List<UploadMemberGroupDto> membergroup = new();
            HashSet<int> invalidRecords = new();

            using var reader = new StreamReader(input.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            try
            {
                membergroup = csv.GetRecords<UploadMemberGroupDto>().ToList();
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid header(s) name");
            }

            for (int i = 0; i < membergroup.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(membergroup[i].Group))
                {
                    errors.Add($"membergroup[{i}]", "membergroup is required");
                    invalidRecords.Add(i);
                }
            }

            var validRecords = membergroup.Where((membergroup, index) => !invalidRecords.Contains(index)).ToList();
            var valideRecordGroup = validRecords.Select(x => x.Group).ToList();
            var existingRecord = _repository.PremiumPlans.FindByCondition(x => valideRecordGroup.Contains(x.Duration), false).Select(x => x.Duration).ToList();

            for (int i = 0; i < existingRecord.Count; i++)
            {
                var index = membergroup.FindIndex(x => x.Group == membergroup[i].Group);

                if (index > -1)
                {
                    errors.Add($"membergroup[{i}]", $"membergroup '{membergroup[i]}' already exist");
                    invalidRecords.Add(i);
                }
            }

            var membergroupDto = membergroup.Where((membergroup, index) => !invalidRecords.Contains(index)).ToList();
            // Mapping the upload details to database parameters
            var membergroupLists = _mapper.Map<List<MemberGroup>>(membergroupDto);

            await _repository.MemberGroup.AddRangeAsync(membergroupLists);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<List<MemberGroupResponseDto>>(membergroupLists);

            return new SuccessResponse<UploadMemberGroupResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = new UploadMemberGroupResponseDto
                {
                    Errors = errors,
                    MemberGroup = response,
                    Message = $"{membergroupLists.Count} record(s) saved."
                }
            };
        }
    }
}

