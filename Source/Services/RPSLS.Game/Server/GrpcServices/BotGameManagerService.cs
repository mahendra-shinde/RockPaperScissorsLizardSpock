﻿using GameBff.Proto;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace RPSLS.Game.Server.GrpcServices
{
    public class BotGameManagerService : GameBff.Proto.BotGameManager.BotGameManagerBase
    {
        private readonly GameApi.Proto.BotGameManager.BotGameManagerClient _botGameManagerClient;
        private readonly ILogger<BotGameManagerService> _logger;

        public BotGameManagerService(GameApi.Proto.BotGameManager.BotGameManagerClient botGameManagerClient, ILogger<BotGameManagerService> logger)
        {
            _botGameManagerClient = botGameManagerClient;
            _logger = logger;
        }

        public override async Task<GameBff.Proto.ChallengersList> GetChallengers(GameBff.Proto.Empty request, ServerCallContext context)
        {
            var challengersListApi = await _botGameManagerClient.GetChallengersAsync(new GameApi.Proto.Empty(), GetRequestMetadata());
            var result = new ChallengersList();
            foreach (var challenger in challengersListApi.Challengers)
            {
                result.Challengers.Add(new ChallengerInfo { Name = challenger.Name, DisplayName = challenger.DisplayName });
            }

            result.Count = result.Challengers.Count;

            return result;
        }

        public override async Task<GameResponse> DoPlay(GameRequest request, ServerCallContext context)
        {
            var playRequest = new GameApi.Proto.GameRequest()
            {
                Challenger = request.Challenger,
                Username = request.Username,
                TwitterLogged = request.TwitterLogged,
                Pick = request.Pick
            };

            var result = await _botGameManagerClient.DoPlayAsync(playRequest, GetRequestMetadata());
            return new GameResponse()
            {
                Challenger = result.Challenger,
                ChallengerPick = result.ChallengerPick,
                Result = (Result)(int)result.Result,
                User = result.User,
                UserPick = result.UserPick,
                IsValid = result.IsValid
            };
        }


        protected Grpc.Core.Metadata GetRequestMetadata()
        {
            var metadata = new Grpc.Core.Metadata();
            return metadata;
        }
    }
}
