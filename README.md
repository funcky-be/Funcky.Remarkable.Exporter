# Funcky.Remarkable.Exporter
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=FER3YNBKV9DEA)

Export notes from a Remarkable Tablet to File System, Evernote, ...

## Disclaimer

This is a hobby project and before any technical information, I have to start with a small disclaimer.

**USE IT AT YOUR OWN RISK**

I only give you here some tips and tricks that I use to backup my Remarkable Tablet because I think this is useful.
I don't give any support, I'm not responsible of any data loss that may occur.

_The author(s) and contributor(s) are not associated with reMarkable AS, Norway.
**reMarkable** is a registered trademark of *reMarkable AS* in some countries.
Please see https://remarkable.com for their product._

And now, let's start with the fun !

# How does it works ?

The program runs some steps and use the Remarkable Api to get the files, and then process all the .lines to draw a PNG.
After these steps, you can do whatever you want with the PNG, in my case, I send it to Evernote.

## Step 0 : Requirements and setup
This project is in .Net, obviously, you need the .Net Framework.
Also, it requires a local storage to store the template, configuration files, ...

All the setup is in two files, the app.config and the devices.json

1. Templates : Grab the templates.zip in the Files folder and extract into a directory
2. devices.json : Create a devices.json file, see the documented sample below
3. App.config : Open the app.config and set the path to the template directory and to the devices.json file.

Okay, everything is up right now, read the steps below to understand how the program really works.

## Step 1 : Device Registration
I use the Remarkable Api like the desktop software, so I have to enroll my virtual device like anything else.
Two informations are required : 
* deviceId : A guid for your virtual device, you have to generate one in the default .Net format
* code : An OTP code that you can get on my.remarkable.com

This step make a request to generate an authentication bearer and store it into the devices.json file.

## Step 2 : Synchronize notes
This step logs into the Remarkable Api with the authentication bearer to get a session bearer.
Then, it gets the list of all the notes on the table.
Each note is referenced by a guid, the program check if the note exists in the local storage. The file is downloaded into the folder with the following structure :
```
<localPath>\<guid[0]>\<guid[1]>\<guid[2]>\<guid[3]>\<note version on four digits>\content.zip
```

The zip file are then extracted in a "content" subdirectory, it contains the Remarkable files in their proprietary format.

## Step 3 : Drawing notes
Yes ... hardest part ... all the notes use a proprietary format, the .lines file contains all the data of the note.
Thanks to some articles on the internet (see the references below), I was able to read it and to draw it on a PNG.
Each page is drawn in a single PNG and they are stored in a "png" subdirectory in the "content" directory.

The drawing is not perfect, see the known issues below, but is readable. It's enough for the main purpose of the project : having a backup location outside of Remarkable data centers.

Fun Fact : When I knew the .lines format, I faced an issue with the drawing library : there is a lot of segment on each page and most of the libraries were very bad in performance !
I used SkiaSharp and it works well, this lib is able to manage all those segments !

## Step 4 : External Services (aka Evernote for me)
Now, we have the PNG, we can do what we want with that file.
I chose to send it to Evernote (Built-in Image OCR and fast search) and I did it the most straightforward way : Send an email to my Evernote inbox with the PNG in the attachments.
The drawback is that I get a note in Evernote for every version on the Remarkable, but one more time, the main purpose of the project is only the backup.

This part is a work in progress, I would like to extend it to manage external services like plugins, maybe a future version :)

## Step 5 : Enjoy and have a beer !
My notes are safe, if anything happen to Remarkable company, I have my notes and I'm happy with that !

# Technical information

## Documented devices.json
To configure the workers, you have to use a json file and reference in the app.config file.

```
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

## Known Issues
Yes, there is some know issues :'(
The most important is that the drawing is not perfect, I try to improve it but, yeah, I'm not a drawer, I'm  developer :)
Any idea to improve this is welcome, the biggest issue is with the pencil ...

## References
A big thank to all the following resources that helped me to build this project :
* splitbrain/ReMarkableAPI : https://github.com/splitbrain/ReMarkableAPI
* Work in progress: Format of the .lines files : https://www.reddit.com/r/RemarkableTablet/comments/7c5fh0/work_in_progress_format_of_the_lines_files/
* reMarkable .lines File Format : https://plasma.ninja/blog/devices/remarkable/binary/format/2017/12/26/reMarkable-lines-file-format.html

## Buy Me A Beer
If this project help you, you can give me a beer :) 

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=FER3YNBKV9DEA)

# One More Time : Disclaimer

**USE IT AT YOUR OWN RISK**

I'm not affiliated to Remarkable, I do reverse engineering to make this project. I also use their proprietary api, read their proprietary format.
I never asked them anything, I do this because they don't give a public API at the moment.
The program never delete anything on their server, but I don't know if they will block my program ...