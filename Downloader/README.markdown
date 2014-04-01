[4chandownloader][1] - a 4chan image downloader tool
=======================================================================

4chandownloader has the following:

* Any board 
* Allows you to specify a 4chan download location for each board
* Multithreaded downloading of board threads
    * Each board has its own thread
    * Each thread downloads images in its own thread
* Option to create a new folder for each thread on the board
    * Deletes empty thread folders automatically
    * Deletes any image files that failed to download successfully
* Only downloads images that are not already contained in the thread folder
* Downloads JPEG, PNG, GIF images from any board
* Sleeper function which allows each board to wait for a given time before it scrapes.
* Timer function allows you to configure how long a board should run.
    * Can be disabled 
    * Timers can be specified for each board seperately
* Console output

First, check out the project completely with the following:

    git clone https://github.com/pjmagee/4chandownloader-v2.git

Open the project in Visual Studio 2010 and Build it

If you wish to use a pre-compiled version (Downloader-Beta.zip) you can get it from the [downloads][2] section

#### Example:

If you downloaded the Downloader-Beta folder and are not building this project yourself, extract the zip file and setup the configuration file.

     config.xml
     
     
You may add as many board elements as you like. 

If you would like a board to *sleep* after it has finished its first process of downloading threads and images you can define a sleep time. The sleep value is in milliseconds.

##### Example:

Changing the board sleep value of 25 seconds

    <board ... snooze="25" ... /> (25 seconds) to <board ... snooze="30" ... /> (30 seconds)
    
If you would like to keep a boards configuration but would not like the board to be included in the downloader.

You would change the disabled value

    disabled="false" (This board will run!) to disabled="true" (This board will not run!)
    
Changing the pages value is not recommended. The pages value is used to scrape from pages 1 to 5 of the board. If you increase this value you may end up with thread folders containing 1 or 2 images which are about to 404
or be moved up to page 1. Which means it would be scraped eventually anyway.

If you would like to use a timer for a board

    <board name="a" pages="5" folders="true" continuous="true" directory="C:\Temp\" disabled="true" snooze="30">
      <timer days="0" hours="0" minutes="0" seconds="0" disabled="true" />
      <constraint minimum="0" disabled="true" />
    </board>
    
Make sure that the disabled="true" is set to "false":

     <timer days="0" hours="0" minutes="0" seconds="0" disabled="false" />

If you wanted board *a* to only run for 2 minutes, use the following:

	<board name="a" pages="5" folders="true" continuous="true" directory="C:\Temp\" disabled="true" snooze="30">
      <timer days="0" hours="0" minutes="2" seconds="0" disabled="false" />
      <constraint minimum="0" disabled="true" />
    </board>
    
If you wanted board a to run constantly, you would use the following: 
     
     <board name="a" pages="5" folders="true" continuous="true" directory="C:\Temp\" disabled="true" snooze="30">
      <timer days="0" hours="0" minutes="2" seconds="0" disabled="true" />
      <constraint minimum="0" disabled="true" />
    </board>
	
If you only wanted a thread to be downloaded that contained more than 5 images, use the following:

	<board name="a" pages="5" folders="true" continuous="true" directory="C:\Temp\" disabled="true" snooze="30">
		<timer days="0" hours="0" minutes="2" seconds="0" disabled="true" />
		<constraint minimum="5" disabled="false" />
    </board>

#### Example directory 

* **b/**
    * **387400490/**
        * 1238547392378.jpg
        * 1238653292371.gif
        * 1425473926233.png
        * ***all images for this thread inside here!***
    * **387400424/**
        * ***all images for this thread inside here!***   
   
* **a/**
    * **387400490/**
         * ***all images for this thread inside here!***
    * **387399346/**
         * ***all images for this thread inside here!***
* **c/**
    * **387385321/**
        * ***all images for this thread inside here!***
    * **387385320/**
         * ***all images for this thread inside here!***
    * **423523224/**
         * ***all images for this thread inside here!***

    

***Once the process has finished (given that no boards have repeat set to true and the timer is disabled), it will stop and give you some information about how many images were downloaded and if you would like to repeat the process.***



##### ** Selecting too many boards and pages can slow down the performance of your connection considerably! **

Update 3-31-14
	Audune:
			-Fixed the naming issue with the new image content servers.  Now downloads properly again.
			-Increased the Image download threads from 5 to 10, because 'Murica
			-Console now prints what Thread it is downloading from, rather than just the generic server that the image lives on
			-Possibly other tweaks I forgot about, at least it works again.

- - -
#### License

    This file is part of 4ChanDownloader.
    For more information visit http://github.com/4ChanDownloader/
    
    Copyright (C) 2012 Patrick Magee
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.
    
    You should have received a copy of the GNU General Public License
    along with this program. If not, see <http://www.gnu.org/licenses/>.




 

[1]: https://github.com/pjmagee/4chandownloader
[2]: https://github.com/pjmagee/4chandownloader/downloads

