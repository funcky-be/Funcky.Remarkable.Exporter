# Funcky.Remarkable.Exporter
Synchronize notes from a Remarkable Tablet to File System, Evernote, ...

## Disclaimer

This is a hobby project and before any technical information, I have to start with a small disclaimer.

**USE IT AT YOUR OWN RISK**

I only give you here some tips and tricks that I use to backup my Remarkable Tablet because I think this is usefull.
I don't give any support, I'm not responsable of any data losse that may occur.

_The author(s) and contributor(s) are not associated with reMarkable AS, Norway.
**reMarkable** is a registered trademark of *reMarkable AS* in some countries.
Please see https://remarkable.com for their product._

And now, let's start with the fun !

# Technical information

**WORK IN PROGRESS**

## Sample devices.json
To configure the workers, you have to use a json file and reference in the app.config file.

### Documented devices.json
```json
{
  "devices": [
    {
      "name": "Sample Device", // Name for your device
      "authenticationBearer": "", // Auto generated
      "code": "abcdefgh", // 8 letters code from my.remarkable.com
      "deviceDesc": "desktop-windows", // Leave it like that
      "deviceID": "00000000-0000-0000-0000-000000000000", // A guid for your virtual device, generate one in the default .Net format
      "evernoteDestinationEmail": "xxx.yyy@m.evernote.com", // Email to your Evernote Account, required only for Evernote synchronisation
      "evernoteNotebook": "MyRemarkableNotes", // Target Evernote Notebook
      "evernoteSourceEmail": "somebody@you.com", // Sender of the email to Evernote
      "localPath": "C:\\tmp\\Remarkable\\simon\\", // Local path to store the data from Evernote
      "sessionBearer": "" // Auto generated
    }
  ],
  "smtp": { // Smtp configuration for all outgoing emails
    "enableSsl": true, // Use SSL to connect to the server
    "host": "smtp.you.com", // Smtp Address
    "port": 587, // Port to connect to the smtp
    "password": "aaaaaaaaaa", // Password if authentication is required
    "userName": "somebody@you.com", // UserName if authentication is required
    "delay": 5 // Delay between emails to throttle, because some server dislike the first sync :)
  }
}
```
