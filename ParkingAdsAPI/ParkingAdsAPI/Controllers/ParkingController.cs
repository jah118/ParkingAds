using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ParkingAdsAPI.Data;
using ParkingAdsAPI.DTO;
using ParkingAdsAPI.RabbitMQs;
using ParkingAdsAPI.Util;


namespace ParkingAdsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : ControllerBase
    {
        private readonly IMessageProducer _messagePublisher;
        private readonly IMessageConsume _messageConsume;

        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;


        public ParkingController(IMessageProducer messagePublisher, IMessageConsume messageConsume,
            IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _messagePublisher = messagePublisher;
            _messageConsume = messageConsume;
            _appSettings = optionsMonitor.CurrentValue;
            //_config = config;
        }


        [HttpGet("{place}", Name = "GetParking")]
        public async Task<IActionResult> GetParking(string place)
        {
            //TODO build MSG that follow Schema 


            var json = new JsonModel
            {
                SearchedLocation = place,
                TopicKey = _appSettings.RabbitQueueNameProduce,

                // Sesssion = null,
                Sesssion = new Sesssion
                {
                    TimeSent = DateTime.Now.ToString(),
                    MessageId = Guid.NewGuid().ToString(),
                    AggregatorTarget = null,
                    SplitCounter = -1
                },
                AdData = null,
                ParkingData = null
            };

            _messagePublisher.SendMessage(json);

            //TODO: call cache 
            //TODO: Await svar  og build return model 
            //TODO: handle callback better 
            var obj = await _messageConsume.GetMessage();


            return Ok(obj);
        }


        [HttpPost]
        public async Task<IActionResult> BookParking([FromBody] ParkingDTO modelDTO)
        {
            ParkingModel model = new()
            {
                Data = modelDTO.Data,
            };


            _messagePublisher.SendMessage(model.Data);


            //TODO: call cache 
            //TODO: Await svar  og build return model 

            return Ok(model);
        }
    }
}