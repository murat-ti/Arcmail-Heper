Archelp

This program written on C# for securely exchanging files between company branches. 
For example: Company A branch have 2 folders (in and out folders) on FTP. They want send their files in secure way to Company B branch, so they put their files into in folder. For accepting files from Company B branch
they have out folder. In the same way Company B branch has 2 folders (in and out) for sending and receiving files. Every brach has their own code (in this program it is 4 chars).

For encrypting and decrypting files I used Arcmail program. This program helps to encrypt and decrypt files with branches codes. It has open and private keys. On C# program i call arcmail through command line
and using 2 command, you can find them in source code.

In this program you can find following:
1. Work with text files (read txt files in table), you can also add some extra action as write.
2. Work with directories and subdirectories (create, loop for searching files with pattern, remove)
3. FTP (uploading and downloading files from your ftp server). Also do some operations (create, move, delete files in FTP)
4. Encrypt/decrypt files. There is you can find only Arcmail program encrypting and decrypting commands. But you can you any other program.
5. Zip and unzip files and directories. I used SharpCompress library for that.
6. Put all dynamic information in config file. For example in future you can change ftp adress or ftp credentials. Also I put here patterns for looking specified files and other things.
7. Write each operation in log text file, so yuo can easily tracking what happens in programm and easily find bugs.

Thanks for attention.
