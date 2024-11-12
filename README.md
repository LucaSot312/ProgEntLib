
# ProgEntLib - Gestione Catalogo Librario

Questa applicazione web API è progettata per la gestione di un catalogo di una libreria, 
con funzionalità per la gestione di utenti, libri e categorie, implementata con .NET 8 e MongoDB.

## Requisiti

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) come cluster io ho utilizzato MongoDB dato che avevo un cluster attivo dall'esame di Ingegneria del Software
- **Nota**: Per il backup e restore dei dati senza installare `mongodump` e `mongorestore`, la gestione è integrata nel codice e utilizza un file JSON per serializzare/deserializzare i dati.

## Configurazione

1. **Clona il repository**:
   ```bash
   git clone https://github.com/LucaSot312/ProgEntLib.git
   cd ProgEntLib
   ```

2. **Configurazione di `appsettings.json`**: il file `appsettings.json` contiene i dettagli di connessione a MongoDB e i parametri JWT, come segue:

   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore.Authentication": "Debug",
         "Microsoft.AspNetCore.Authorization": "Debug"
       }
     },
     "AllowedHosts": "*",
     "MongoDB": {
       "ConnectionString": "<CONNECTION_STRING>",
       "DatabaseName": "LibreriaEnterprise",
       "BackupSettings": {
         "BackupFolderPath": "Backups"
       },
       "CollectionNames": {
         "Utenti": "Utenti",
         "Libri": "Libri",
         "Categorie": "Categorie"
       }
     },
     "JwtSettings": {
       "SecretKey": "<SECRET_KEY>",
       "ExpiryInMinutes": 60,
       "Issuer": "LibraryCatalogAPI",
       "Audience": "LibraryCatalogAPIClient"
     }
   }
   ```

   - **ConnectionString**: Stringa di connessione a mongodb contenente Username, password e nome cluster.
   - **BackupFolderPath**: Specifica la cartella per il backup, già impostata su `Backups` all'interno del progetto.
   - **JwtSettings.SecretKey**: Nella `<SECRET_KEY>` va inserita una stringa di almeno 32 caratteri con maiuscole minuscole numeri e simboli

3. **NuGet Packages**: Il progetto gestisce automaticamente i pacchetti necessari; se necessario, aggiorna le dipendenze con il comando:
   ```bash
   dotnet restore
   ```

## Avvio dell'applicazione

`dotnet run --launch-profile https`

L'applicazione sarà disponibile all'indirizzo `https://localhost:7262/swagger` (oppure http://localhost:5218/swagger) secondo quanto configurato su `launchSettings.json`.

## Testing dell'API

### Autenticazione

1. **Registrazione Utente**: Usa l'endpoint `/api/utenti/registrazione` per creare un nuovo utente.
2. **Generazione Token**: Usa l'endpoint `/api/utenti/login` per ottenere un token JWT. 
3. **Autenticazione Utente** Inserisci il token JWT ottenuto in Swagger o nel terminale dell'IDE nel campo Authorize in alto a destra 
     per autenticare le richieste. **Consiglio** per evitare errori di formattazione incollare con `CTRL+SHIFT+V`.

### Test delle API

L'applicazione è documentata con Swagger.

Endpoint disponibili:

- **Categorie**:
  - POST `/api/categorie` - Crea una nuova categoria.
  - DELETE `/api/categorie/{id}` - Elimina una categoria (solo se priva di libri).
- **Libri**:
  - POST `/api/libri` - Aggiungi un nuovo libro.
  - PUT `/api/libri/{id}` - Modifica un libro esistente.
  - DELETE `/api/libri/{id}` - Elimina un libro.
  - GET `/api/libri` - Cerca libri con filtri (categoria, nome, autore, ecc.) e paginazione.
- **Backup e Restore del Database**:
  - POST `/api/database/backup` - Effettua il backup del database. I file JSON per ogni tabella vengono salvati nella cartella `Backups`.
  - POST `/api/database/restore` - Ripristina il database da un file JSON nella cartella `Backups`tramite il proprio nome, recuperando la tabella.

## Backup e Restore del Database

### Backup

L'endpoint `/api/database/backup` genera i file JSON nella cartella `Backups` contenenti il dump dell'intero database. Questo file può essere caricato in altri ambienti o usato per il ripristino.

1. Esegui una richiesta POST a `/api/database/backup` :
   ```bash
   curl -X POST "https://localhost:5001/api/database/backup"
   ```

### Restore

L'endpoint `/api/database/restore` consente di ripristinare il database dal file di backup.

1. Inserisci il nome del file da ripristinare (ad es. `LibreriaEnterprise-backup-2024-11-07.json`).
2. Esegui una richiesta POST a `/api/database/restore`, specificando il nome del file:
   ```bash
   curl -X POST "https://localhost:5001/api/database/restore" -d "nomeFile=LibreriaEnterprise-backup-2024-11-07"
   ```

**Nota**: I file di backup devono trovarsi nella cartella `Backups` del progetto.

## Problemi Comuni

- **JWT non valido**: Se l'autenticazione fallisce, verifica che il token JWT sia stato copiato correttamente anche nella formattazione.
- **Porta occupata**: Se la porta che ho definito sul `launchsettings.json` risulta occupata da un altro processo sulla macchina cambiare valore nel file
