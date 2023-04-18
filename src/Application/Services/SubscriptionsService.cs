using Application.Contracts;
using Application.Helpers;
using AutoMapper;
using Infrastructure.Contracts;
using Shared;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SubscriptionsService: ISubscriptionsService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        public SubscriptionsService(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SuccessResponse<IEnumerable<SubscriptionResponseDto>>> GetAllSubscriptionsAsync()
        {

            var response = new SuccessResponse<IEnumerable<SubscriptionResponseDto>>();

            var result =  await _repository.Subscriptions.GetAllAsync();
            var subscriptions = result.Select(x => new SubscriptionResponseDto
            {
                Id = x.Id,
                SubscriptionPlan = x.SubscriptionPlan
              
            }).ToList();

            if (!subscriptions.Any())
                throw new RestException(HttpStatusCode.NotFound, $"No Subscription available");

            response.Message = ResponseMessages.DataRetrievedSuccessfully;
            response.Data = subscriptions;

            return response;
            
        }

        public async Task<SuccessResponse<IEnumerable<PremiumPlansResponseDto>>> GetPremiumPlansAsync()
        {

            var response = new SuccessResponse<IEnumerable<PremiumPlansResponseDto>>();

            var result = await _repository.PremiumPlans.GetAllAsync();
            var premiumplans = result.Select(x => new PremiumPlansResponseDto
            {
                PremiumId = x.PremiumId,
                Duration = x.Duration,
                Amount = x.Amount,
                TotalAmount = x.TotalAmount,
                PercentSavings = x.PercentSavings,
                Savings = x.Savings

            }).ToList();

            if (!premiumplans.Any())
                throw new RestException(HttpStatusCode.NotFound, $"No Subscription available");

            response.Message = ResponseMessages.DataRetrievedSuccessfully;
            response.Data = premiumplans;

            return response;
        }
    }
}
