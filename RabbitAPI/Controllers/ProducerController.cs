using Microsoft.AspNetCore.Mvc;
using RabbitAPI.RabbitMQ;
using RekrutacjaLib;
using System.Net.Mail;
using System;

namespace RabbitAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly IRabbitMQProducer _rabitMQProducer;

        public ProducerController(IRabbitMQProducer rabbitMQProducer)
        {
            _rabitMQProducer = rabbitMQProducer;
        }

        [HttpPost]
        public IActionResult PostToRabbit(Mail mail)
        {
            try
            {
                var emailAddress = new MailAddress(mail.To);
            }
            catch
            {
                return StatusCode(400);
            }

            _rabitMQProducer.SendProductMessage(mail);
            return Ok(mail);
        }
    }
}
