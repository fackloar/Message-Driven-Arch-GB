using ConsoleApp.Models;
using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var rest = new Restaraunt();
var speakerBot = new SpeakerBot(rest);

speakerBot.InitialHello();

