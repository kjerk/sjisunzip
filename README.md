sjisunzip
=========

This is a pretty braindead command line utility that simply forces the encoding to the right values to extract a Shift JIS encoded zip file ('[Code page 932](http://en.wikipedia.org/wiki/Code_page_932)') on a western/ansi encoding system.

[Download Here](https://github.com/kjerk/sjisunzip/releases)

```
Usage:
  sjisunzip someFile.zip [toFolder]
  sjisunzip [-r] someFile.zip
    -r: Recode file to {filename}_utf8.zip
Examples:
  sjisunzip aFile.zip
  sjisunzip aFile.zip MyNewFolder
```

You can also just drop a zip file onto the program since that'll pass it as the first argument and the contents will be extracted in the same directory.

If you've ever recieved a zip file from a friend or the wrong damn gnu mirror or whatever that passed through Japan then you've probably seen garbled filenames
![example_1](https://cloud.githubusercontent.com/assets/2738686/5326938/37acc0de-7ce7-11e4-8259-06ef8b1f43a8.jpg)
---

Well this program forces the opened zip to the correct encoding then extracts the file to a more reasonable UTF encoding.
![example_2](https://cloud.githubusercontent.com/assets/2738686/5326978/712d7e50-7ce9-11e4-8f18-c885afc51055.jpg)
---

You can even just reencode the zip file to a less busted-ass one so you don't have this creeping horror issue in the future
![example_3](https://cloud.githubusercontent.com/assets/2738686/5326937/37ab2878-7ce7-11e4-9655-61b92a2b680d.jpg)
---

The filenames and paths should be untangled when done.
![example_4](https://cloud.githubusercontent.com/assets/2738686/5326940/37af9d72-7ce7-11e4-8ee2-3a9d11c6e669.jpg)
