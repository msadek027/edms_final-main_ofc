//
// Dynamsoft JavaScript Library for Basic Initiation of Dynamic Web TWAIN
// More info on DWT: http://www.dynamsoft.com/Products/WebTWAIN_Overview.aspx
//
// Copyright 2017, Dynamsoft Corporation 
// Author: Dynamsoft Team
// Version: 12.3
//
/// <reference path="dynamsoft.webtwain.initiate.js" />
var Dynamsoft = Dynamsoft || { WebTwainEnv: {} };

Dynamsoft.WebTwainEnv.AutoLoad = true;
///
Dynamsoft.WebTwainEnv.Containers = [{ ContainerId: 'dwtHorizontalThumbnil', Width: 100 + '%', Height: 200 },
{ ContainerId: 'dwtVerticalThumbnil', Width: 150, Height: 500 },
{ ContainerId: 'dwtLargeViewer', Width: 755, Height: 500 },
{ ContainerId: 'dwtQuickViewer2', Width: 80 + '%', Height: 500 },
{ ContainerId: 'dwtQuickViewer', Width: 80 + '%', Height: 500 }];
///
Dynamsoft.WebTwainEnv.ProductKey = 'D2E369E31929C2C4F9C36AC13BE2DF2911CECCA19F57E9EE3DEE32A4D87D1B3DAA005C717E05ECE6C71E9113CC9E7A1C7F665194B0590F30BF0C7D3FC44B62696DE482C66394D4C7B9B4D678FC3016D89A1C77EF8FBB46D5FFC0EE454822DE7A6F8125190E769F7A372876A60ACE067378;D5400E9A8FA2708D1F28E2349D45FA3B329EDD9A30AEDA3FD4BE3F513AB6CEDF6A80AB875878B9271A49FCD3EBC1ADE0E154EDAE4D1740F560E7970031A79A9B45F70265104FC2C6DA1679AB9897174548FD1384157AB788505DBB436FC3DE555D7E8863EED72DFE114B0F0A';
///
Dynamsoft.WebTwainEnv.Trial = false;
///
Dynamsoft.WebTwainEnv.ActiveXInstallWithCAB = false;
Dynamsoft.WebTwainEnv.Debug = false;
Dynamsoft.WebTwainEnv.ResourcesPath = '../Resources';
///
// Dynamsoft.WebTwainEnv.ResourcesPath = 'Resources';

/// All callbacks are defined in the dynamsoft.webtwain.install.js file, you can customize them.

// Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', function(){
// 		// webtwain has been inited
// });

