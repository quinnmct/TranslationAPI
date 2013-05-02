<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="TranslationAPI.Welcome"  enableEventValidation="true"%>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!--
    <!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body style="text-align:center;">
    <h1>Welcome to Vernaculate</h1>
    coming soon..
</body>
</html>
-->
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta charset="utf-8" />
        <link href="Content/Site.css" rel="stylesheet" />
       <script src="scripts/soundmanager2.js"></script>
    <script type="text/javascript" 
        src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js">
</script>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server"></telerik:RadAjaxManager>
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server"> 
    
    <div style="text-align:center; background-color:#6B3305;">
        <h1 style="margin-left:-30em; color:#789bc8;">Welcome to Vernaculate</h1>
        <div style="text-align:right;">check out the app!</div>
    </div>
        
    <!--<hr width="100%" style="border: 3px groove color="#FFFFFF" size="6" />  <!-- says error, but really none-->
      <nav style="width:8%; float:left;">
        <div id="langSelections" style="background-color:#A45411; border:groove; ">
            <h3>Select languages below:</h3>
            <fieldset>
                <legend>Select Languages</legend>
                <label><input type="checkbox" value="ar"/>Arabic</label><br/>
                   <label><input type="checkbox" value="bg"/>Bulgarian</label><br/>
                   <label><input type="checkbox" value="ca"/>Catalan</label><br/> 
                   <label><input type="checkbox" value="zh-cn"/>Chinese</label><br/>  
                   <label><input type="checkbox" value="da"/>Danish</label><br/>   
                   <label><input type="checkbox" value="nl"/>Dutch</label><br/>
                 <label><input type="checkbox" value="en"/>English</label><br/>
                   <label><input type="checkbox" value="fi"/>Finnish</label><br/>
	               <label><input type="checkbox" value="fr"/>French</label><br/>
                   <label><input type="checkbox" value="de"/>German</label><br/>
                   <label><input type="checkbox" value="hi"/>Hindi</label><br/>
                   <label><input type="checkbox" value="it"/>Italian</label><br/>
                   <label><input type="checkbox" value="ja"/>Japanese</label><br/>
                   <label><input type="checkbox" value="ko"/>Korean</label><br/>
                   <label><input type="checkbox" value="no"/>Norwegian</label><br/>
                   <label><input type="checkbox" value="pl"/>Polish</label><br/>
                   <label><input type="checkbox" value="pt"/>Portuguese</label><br/>
                   <label><input type="checkbox" value="ru"/>Russian</label><br/>
                   <label><input type="checkbox" value="es"/>Spanish</label><br/>
                   <label><input type="checkbox" value="sv"/>Swedish</label><br/>
                   <label><input type="checkbox" value="th"/>Thai</label><br/>
                   <label><input type="checkbox" value="tr"/>Turkish</label><br/>
            </fieldset>
        </div>
        </nav>

        <section>
          <article style="width:50%;">
           <div id="contentDiv" style="margin-left:10px; margin-top:10px;">
             <telerik:RadTextBox id="wordSearch" runat="server" Text="Enter your phrase here" Width="80em" Height="3.5em"></telerik:RadTextBox>
             <!--<telerik:RadButton id="searchButton" runat="server" Text="Translate" AutoPostBack="false" OnClientClicked="searchButton_Clicked"></telerik:RadButton>-->
            <button id="searchButton">Translate</button>
        
                
                <ul id="translated" data-role="listview" data-type="group" style="list-style-type: none; margin-left:-3em;">
                <h1>Translate Above!</h1>

                <h2><div style="font-size:1.3em; float:left">← </div>
                <div style="float:left;">Select languages to the left</div></h2>
                
                </ul><!--this is where all phrases end up-->
            </div>
          </article>
          <aside id="aside" style="float:right; padding-top:70px;">
              <div id="defineView" style="border-style:groove; margin-left:-55%;">
                 <header> 
                     <div id="fullPhrase"></div> 
                     <div id="currentLang"></div>
                     <div id="ttsDiv" style="float:right; display:none;"><button id="TTS" onclick="thisF()">Speak</button></div>
                 </header>

                  <div id="modalText"></div>

                  
                  <div id="langCode" style="display:none;"></div>
                  <div id="canSpeak" style="display:none;"></div>
               </div>
          </aside>
        </section>
        
        </telerik:RadAjaxPanel>
    </form>

     <script type="text/javascript" id="telerikClientEvents1">
         /////////  TRANSLATE METHODS ///////////
         var phraseArr = [];  //array holding temp translated phrases
         var phraseIndex;     //var to hold index of each phrase
         var defaultLang = "en";

         //'define' button clicked
         $('#appendedButton').live('click', function () {
             
             $("#defineView").append($('#translated li div button').index(e.target) + '<br/>'); 
             return false;
         });
         //define button clicked
         $('#defineButton').live('click', function (e) {
             var index = $('#translated li div button').index(e.target);
            // $("#defineView").empty();
             //clear all data fields in the modal view
             $("#modalText").empty(); $("#headerDiv").empty(); $("#canSpeak").empty();
             $("#fullPhrase").empty(); $("#currentLang").empty(); $("#langCode").empty();
            // $("#ttsDiv").style.display = '';
             $("#fullPhrase").append('<h2>' + phraseArr[index].value + '</h2>');
             $("#currentLang").append(phraseArr[index].langName); $("#langCode").append(phraseArr[index].lang);
             $("#modalText").append('<ul id="translatedWords" data-role="listview"></ul>');
             if (phraseArr[index].canSpeak == false) {
                 $("#canSpeak").append("No");
             } else {
                 $("#canSpeak").append("Yes");
             }
             //THIS WILL NOT WORK with certain characters (ex. japanese) which don't have spaces between them.
             for (i = 0; i < phraseArr[index].words.length; i++) {
                 //each word in here
                 console.log(phraseArr[index].langName);
                 $("#translatedWords").append('<li><div class="wordDiv">' +
                    phraseArr[index].words[i].theWord + ':</div><div class="definitionDiv">'
                   + phraseArr[index].words[i].definition + '</div><br/></li>');
             }

             return false;
         });
         
         $("#searchButton").live('click', searchButton_Clicked);
             //button click event
             //main 'translate' function; checks 'checked' checkboxes, then 
             //calls my TranslationAPI to do the translation
             function searchButton_Clicked(sender, args) {
                 $("#translated").empty(); //empty out the result list
                 $("#modalText").empty(); $("#headerDiv").empty(); $("#canSpeak").empty();
                 $("#fullPhrase").empty(); $("#currentLang").empty(); $("#langCode").empty();
                // $("#ttsDiv").style.display = 'none';
             phraseArr.length = 0; //empty out the temp phrase list
             phraseIndex = -1;
             $("fieldset input:checked").each(function () {
                 var to = this.value;
                 var text = $('#wordSearch').val();
                 //JSON object being sent; untranslated
                 var untranslated = { "value": text, "lang": to };
                 $.ajax({
                     type: "POST",
                     dataType: "json",
                     data: "{}",
                     contentType: "application/json",
                     data: JSON.stringify(untranslated),
                     url: "/api/translate/",
                     success: (addEachPhrase)
                 });
             });

         }
         //add each phrase to the list, formatted accordingly
         function addEachPhrase(response) {
             phraseIndex += 1;
             phraseArr[phraseIndex] = response;
              $("#translated").append('<li><div style="background-color: rgba(120,155,200,0.5);"><p class="floatLeft">'
               + response.langName + '</p><p class="floatRight"><button id="defineButton">Define</button>'
               + '</p><div style="clear: both;"></div></div><div style="font-size:1.8em;">' + response.value + '</div><ul id="'+response.lang+'"></ul></li><br/>');
         }

         ///////////TEXT - TO - SPEECH //////////////
         var mySound;
         /*$("#textToSpeechButton").click(function (e) {*/
         function thisF(e){
             $.ajax({
                 type: "GET",
                 url: "/api/translate/",
                 success: (getTTS)
             });
         }
         //first_response holds access token                    
         function getTTS(first_response) {
             var textToSpeak;
             var langToSpeak;
             console.log($("#canSpeak").text());
             //checks to see if language is supported by TTS service
             if ($("#canSpeak").text() == "No") {
                 textToSpeak = $("#currentLang").text() + " is not supported by text to speech";
                 langToSpeak = defaultLang;
             } else {
                 textToSpeak = $("#fullPhrase").text();
                 langToSpeak = $("#langCode").text();
             }
             var s = document.createElement("script");
             s.src = "http://api.microsofttranslator.com/V2/Ajax.svc/Speak?appId=Bearer "
                   + encodeURIComponent(first_response.access_token) + "&text="
                   + textToSpeak + "&language=" + langToSpeak + "&format=audio/mp3";
             $.ajax({
                 url: s.src,
                 success: function (response) {
                     //response.replace prepares returned URL for passing into sound manager
                     //console.log(response);
                     response = response.replace(/\\/g, ""); //remove all \ characters
                     response = response.replace(/"/g, '');  //remove " from beginning and end 
                     //console.log("HHHHHH:"+response);
                     soundManage(response);
                 }
             });
         }
         function soundManage(response) {
             soundManager.setup({
                 url: 'soundmanager2.swf',
                 onready: function () {
                     // Ready to use; soundManager.createSound() etc. can now be called.
                     soundManager.destroySound('aSound');
                     mySound = soundManager.createSound({
                         id: 'aSound',
                         url: response
                     });
                     mySound.play();
                 }
             });
         }
</script>

</body>
</html>
