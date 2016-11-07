# FastClasses for C++ Visual Studio Extension
## Summary
A simple Visual Studio Extension saving your effort for writing C++ classes.
Too lazy to write all the default class components? This is for you!

## Description
This Visual Studio Extension will add a 3 command buttons to your contextual menu(right click).
By clicking the command a window will appear asking for the class name.
The extension will automatically find the type of the active document and automatically
generate a class definition or a class declaration snippet accordingly.

The available 3 types of commands each generate,

* A basic class with a default constructor, destructor.
* A class with all the above, a copy constructor, copy assign operator.
* A class with all the above, a move constructor, a move assign operator.

version 1.0

This version currently only officially supports Visual Studio 2015(14.0),
I'm not shure if it will work in other versions

## How to use
1. Right Click and open the contextual menu in the document you want to add the snippet.
2. Choose and click the FastClasses command you want according to the type of class you desire.
3. A window asking for the name of the class will show.
4. Write the class name you want.
5. Press enter.
6. Boom! just like that

## Installation
1. Download or clone the repository.
2. Build the project
3. in FastClassesVSIX\FastClassesVSIX\bin\debug you will find FastClassesVSIX.vsix
4. run the file and the the extension will be installed to your Visual Studio

## Contribution
I'm currently a Computer Engineering student actively learning. If you have any feedback about the code,
Simply add an issue on git.
Bug fixes (though I suspect there is any) are strongly welcome.
Any personal questions, sent to msca8h@naver.com
