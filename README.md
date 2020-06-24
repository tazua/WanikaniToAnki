# WanikaniToAnki
This Forms App lets you import all your unlocked Kanji and vocabulary from WaniKani and creates csv documents that you can import into Anki via the Anki desktop application.
Its sloppy code, I know, dont @ me for that.
Any Bug reports are welcome.

## Usage
* Download the latest version [here](https://github.com/tazua/WanikaniToAnki/releases).
* Unpack the zip with [winrar](https://www.google.com/search?q=winrar) or another archive tool.
* Start it by executing the exe file.
* You need a Wanikani API v2 token from [here](https://www.wanikani.com/settings/personal_access_tokens) (need to be logged in for this link to work). It doesnt need any write access so if you dont have a token already, click "Generate a new token" enter any token description and click generate token (no checkboxes needed).
* Copy paste the token into the textfield (APIv2 Token).
* If you have anki already installed you can choose your profile if you click the anki profiles button. The programm will then use that profile to download images into the collection.media folder, which anki uses to import media into cards. if no other other iage folder is specified it will create a "img" folder in its directory.
* click start and wait for the process to finish. this can take a while, depending on how many kanji and vocabulary you have already learned, because of the wanikani apis rate limit. for all runs after the initial run, the programm will load all already downloaded data from the generated csv documents so it wont take as long updating. when it says done and shows the amount of added kanji and vocabulary, you can close the app.
* the downloaded csv files are in the "csv" folder. one for the kanji, one for the vocabulary.
* open the anki desktop app. import the csvs into anki via the "import file" button.

## CSV Format
The CSVs have the following format:

ID,Kanji,primary meaning,secondary meanings,primary reading,secondary readings,stroke order

(stroke order will be empty in the vocabulary csv)
