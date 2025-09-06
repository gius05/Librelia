using Librelia.Database;
using Librelia.Models;
using Librelia.Repositories;
using MongoDB.Driver;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Librelia.Services;

namespace Librelia.Services
{
    public class CheckExpiredBooksService : BackgroundService
    {
        private readonly ILogger<CheckExpiredBooksService> _logger;
        private readonly IMongoCollection<Reservation> _reservationRepository;
        private readonly EmailService _emailServece;
        private readonly BookRepository _bookRepository;
        private readonly UserRepository _userRepository;
        public CheckExpiredBooksService(ILogger<CheckExpiredBooksService> logger, MongoDBContext database, EmailService emailService, BookRepository bookRepository, UserRepository userRepository)
        {
            _logger = logger;
            _reservationRepository = database.GetCollection<Reservation>("Reservations");
            _emailServece = emailService;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndUpdateExpiredItems();
                    await SendEmailToExpiredPrenotation();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"(Reservation) Errore durante il controllo degli elementi scaduti: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Esegue il controllo ogni ora
            }
        }

        private async Task CheckAndUpdateExpiredItems()
        {
            var now = DateTime.UtcNow;
                         //Se é scaduta   
            var filter = Builders<Reservation>.Filter.Lt(x => x.Expire_Date, now) 
                         // Se é passato almeno un giorno dal ultima mail inviata
                         & Builders<Reservation>.Filter.Ne(x => x.Status, "scaduto");
            var update = Builders<Reservation>.Update.Set(x => x.Status, "scaduto").Set(x => x.MailSent_Date, DateTime.Now);
            
            var reservations = await _reservationRepository.Find(filter).ToListAsync();
            
            
            
            reservations.ForEach(async reservation =>
            {
                var book = await _bookRepository.GetById(reservation.BookId);
                var user = await _userRepository.GetByEmail(reservation.Email);
                
                // Il body della mail è in formato HTML
                var mailBody = $@"
                    <!DOCTYPE html>
                    <html lang=""it"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Scadenza superata!</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 20px;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: auto;
                                background: #ffffff;
                                padding: 20px;
                                border-radius: 5px;
                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            }}
                            h1 {{
                                color: red;
                            }}
                            p {{
                                color: #555;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #777;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <h1>Avviso!</h1>
                            <p>Ciao {user.Name + " " + user.Surname},</p>
                            <p>Vogliamo informarti che il libro: <strong>{book.Title}</strong> (ISBN: {book.Isbn})</p>
                            <p>di: {string.Join(", ", book.Authors)}</p>
                            <p>Edito da: {book.House},</p>
                            <p> che hai prenotato ha superato la data di riconsegna.</p>
                            <p>Per evitare possibili ripercusioni ti invitiamo a restuirlo il piú presto possible</p>
                            <div class=""footer"">
                              <p>Grazie mille per la comprensione, un saluto dallo staff di Librelia.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
                await _emailServece.SendMailAsync(reservation.Email, "Prenotazione Scaduta", mailBody);
                
            });
            var result = await _reservationRepository.UpdateManyAsync(filter, update);

            _logger.LogInformation($"(Reservation) Elementi scaduti aggiornati: {result.ModifiedCount}");
        }

       private async Task SendEmailToExpiredPrenotation()
        {
                        // Se é passato almeno un giorno dal ultima mail inviata
            var filter = Builders<Reservation>.Filter.Eq(x => x.Status, "scaduto")
                         & Builders<Reservation>.Filter.Lt(x => x.MailSent_Date, DateTime.UtcNow.AddDays(-1));
            var update = Builders<Reservation>.Update.Set(x => x.MailSent_Date, DateTime.Now);

            var reservations = await _reservationRepository.Find(filter).ToListAsync();
            
            
            
            reservations.ForEach(async reservation =>
            {
                var book = await _bookRepository.GetById(reservation.BookId);
                var user = await _userRepository.GetByEmail(reservation.Email);
                
                // Il body della mail è in formato HTML
                var mailBody = $@"
                    <!DOCTYPE html>
                    <html lang=""it"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Scadenza superata!</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 20px;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: auto;
                                background: #ffffff;
                                padding: 20px;
                                border-radius: 5px;
                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            }}
                            h1 {{
                                color: red;
                            }}
                            p {{
                                color: #555;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #777;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <h1>Avviso!</h1>
                            <p>Ciao {user.Name + " " + user.Surname},</p>
                            <p>Vogliamo informarti che il libro: <strong>{book.Title}</strong> (ISBN: {book.Isbn})</p>
                            <p>di: {string.Join(", ", book.Authors)}</p>
                            <p>Edito da: {book.House},</p>
                            <p> che hai prenotato ha superato la data di riconsegna.</p>
                            <p>Per evitare possibili ripercusioni ti invitiamo a restuirlo il piú presto possible</p>
                            <div class=""footer"">
                              <p>Grazie mille per la comprensione, un saluto dallo staff di Librelia.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
                await _emailServece.SendMailAsync(reservation.Email, "Prenotazione Scaduta", mailBody);
            });
            var result = await _reservationRepository.UpdateManyAsync(filter, update);

            _logger.LogInformation($"(Reservation) Email inviate alle prenotazione scadute: {result.ModifiedCount}");
        }
    }
}
