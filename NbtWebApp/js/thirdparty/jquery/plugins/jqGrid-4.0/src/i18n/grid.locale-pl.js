;(function($){
/**
 * jqGrid Polish Translation
 * Åukasz Schab
 * http://FreeTree.pl
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "PokaÅ¼ {0} - {1} z {2}",
	    emptyrecords: "Brak rekordÃ³w do pokazania",
		loadtext: "\u0142adowanie...",
		pgtext : "Strona {0} z {1}"
	},
	search : {
	    caption: "Wyszukiwanie...",
	    Find: "Szukaj",
	    Reset: "CzyÅ›Ä‡",
	    odata : ['dok\u0142adnie', 'rÃ³Å¼ne od', 'mniejsze od', 'mniejsze lub rÃ³wne','wiÄ™ksze od','wiÄ™ksze lub rÃ³wne', 'zaczyna siÄ™ od','nie zaczyna siÄ™ od','zawiera','nie zawiera','koÅ„czy siÄ™ na','nie koÅ„czy siÄ™ na','zawiera','nie zawiera'],
	    groupOps: [	{ op: "ORAZ", text: "wszystkie" },	{ op: "LUB",  text: "kaÅ¼dy" }	],
		matchText: " pasuje",
		rulesText: " regu\u0142y"
	},
	edit : {
	    addCaption: "Dodaj rekord",
	    editCaption: "Edytuj rekord",
	    bSubmit: "Zapisz",
	    bCancel: "Anuluj",
		bClose: "Zamknij",
		saveData: "Dane zosta\u0142y zmienione! ZapisaÄ‡ zmiany?",
		bYes : "Tak",
		bNo : "Nie",
		bExit : "Anuluj",
	    msg: {
	        required:"Pole jest wymagane",
	        number:"ProszÄ™ wpisaÄ‡ poprawnÄ… liczbÄ™",
	        minValue:"wartoÅ›Ä‡ musi byÄ‡ wiÄ™ksza lub rÃ³wna",
	        maxValue:"wartoÅ›Ä‡ musi byÄ‡ mniejsza od",
	        email: "nie jest adresem e-mail",
	        integer: "ProszÄ™ wpisaÄ‡ poprawnÄ… liczbÄ™",
			date: "ProszÄ™ podaj poprawnÄ… datÄ™",
			url: "jest niew\u0142aÅ›ciwym adresem URL. PamiÄ™taj o prefiksie ('http://' lub 'https://')",
			nodefined : " is not defined!",
			novalue : " return value is required!",
			customarray : "Custom function should return array!",
			customfcheck : "Custom function should be present in case of custom checking!"
		}
	},
	view : {
	    caption: "PokaÅ¼ rekord",
	    bClose: "Zamknij"
	},
	del : {
	    caption: "Usuwanie",
	    msg: "Czy usunÄ…Ä‡ wybrany rekord(y)?",
	    bSubmit: "UsuÅ„",
	    bCancel: "Anuluj"
	},
	nav : {
		edittext: " ",
	    edittitle: "Edytuj wybrany wiersz",
		addtext:" ",
	    addtitle: "Dodaj nowy wiersz",
	    deltext: " ",
	    deltitle: "UsuÅ„ wybrany wiersz",
	    searchtext: " ",
	    searchtitle: "Wyszukaj rekord",
	    refreshtext: "",
	    refreshtitle: "Prze\u0142aduj",
	    alertcap: "Uwaga",
	    alerttext: "ProszÄ™ wybraÄ‡ wiersz",
		viewtext: "",
		viewtitle: "View selected row"
	},
	col : {
	    caption: "PokaÅ¼/Ukryj kolumny",
	    bSubmit: "ZatwierdÅº",
	    bCancel: "Anuluj"	
	},
	errors : {
		errcap : "B\u0142Ä…d",
		nourl : "Brak adresu url",
		norecords: "Brak danych",
	    model : "D\u0142ugoÅ›Ä‡ colNames <> colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"Nie", "Pon", "Wt", "Åšr", "Cz", "Pi", "So",
				"Niedziela", "Poniedzia\u0142ek", "Wtorek", "Åšroda", "Czwartek", "PiÄ…tek", "Sobota"
			],
			monthNames: [
				"Sty", "Lu", "Mar", "Kwie", "Maj", "Cze", "Lip", "Sie", "Wrz", "PaÅº", "Lis", "Gru",
				"StyczeÅ„", "Luty", "Marzec", "KwiecieÅ„", "Maj", "Czerwiec", "Lipiec", "SierpieÅ„", "WrzesieÅ„", "PaÅºdziernik", "Listopad", "GrudzieÅ„"
				],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j < 11 || j > 13 ? ['', '', '', ''][Math.min((j - 1) % 10, 3)] : ''},
			srcformat: 'Y-m-d',
			newformat: 'd/m/Y',
			masks : {
			    ISO8601Long:"Y-m-d H:i:s",
			    ISO8601Short:"Y-m-d",
			    ShortDate: "n/j/Y",
			    LongDate: "l, F d, Y",
			    FullDateTime: "l, F d, Y g:i:s A",
			    MonthDay: "F d",
			    ShortTime: "g:i A",
			    LongTime: "g:i:s A",
			    SortableDateTime: "Y-m-d\\TH:i:s",
			    UniversalSortableDateTime: "Y-m-d H:i:sO",
			    YearMonth: "F, Y"
			},
			reformatAfterEdit : false
		},	
		baseLinkUrl: '',
		showAction: '',
	    target: '',
	    checkbox : {disabled:true},
		idName : 'id'
	}
};
})(jQuery);