;(function($){
/**
 * jqGrid Czech Translation
 * Pavel Jirak pavel.jirak@jipas.cz
 * doplnil Thomas Wagner xwagne01@stud.fit.vutbr.cz
 * http://trirand.com/blog/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "Zobrazeno {0} - {1} z {2} zÃ¡znamÅ¯",
	    emptyrecords: "Nenalezeny Å¾Ã¡dnÃ© zÃ¡znamy",
		loadtext: "NaÄÃ­tÃ¡m...",
		pgtext : "Strana {0} z {1}"
	},
	search : {
		caption: "VyhledÃ¡vÃ¡m...",
		Find: "Hledat",
		Reset: "Reset",
	    odata : ['rovno', 'nerovono', 'menÅ¡Ã­', 'menÅ¡Ã­ nebo rovno','vÄ›tÅ¡Ã­', 'vÄ›tÅ¡Ã­ nebo rovno', 'zaÄÃ­nÃ¡ s', 'nezaÄÃ­nÃ¡ s', 'je v', 'nenÃ­ v', 'konÄÃ­ s', 'nekonÄÃ­ s', 'obahuje', 'neobsahuje'],
	    groupOps: [	{ op: "AND", text: "vÅ¡ech" },	{ op: "OR",  text: "nÄ›kterÃ©ho z" }	],
		matchText: " hledat podle",
		rulesText: " pravidel"
	},
	edit : {
		addCaption: "PÅ™idat zÃ¡znam",
		editCaption: "Editace zÃ¡znamu",
		bSubmit: "UloÅ¾it",
		bCancel: "Storno",
		bClose: "ZavÅ™Ã­t",
		saveData: "Data byla zmÄ›nÄ›na! UloÅ¾it zmÄ›ny?",
		bYes : "Ano",
		bNo : "Ne",
		bExit : "ZruÅ¡it",
		msg: {
		    required:"Pole je vyÅ¾adovÃ¡no",
		    number:"ProsÃ­m, vloÅ¾te validnÃ­ ÄÃ­slo",
		    minValue:"hodnota musÃ­ bÃ½t vÄ›tÅ¡Ã­ neÅ¾ nebo rovnÃ¡ ",
		    maxValue:"hodnota musÃ­ bÃ½t menÅ¡Ã­ neÅ¾ nebo rovnÃ¡ ",
		    email: "nenÃ­ validnÃ­ e-mail",
		    integer: "ProsÃ­m, vloÅ¾te celÃ© ÄÃ­slo",
			date: "ProsÃ­m, vloÅ¾te validnÃ­ datum",
			url: "nenÃ­ platnou URL. VyÅ¾adovÃ¡n prefix ('http://' or 'https://')",
			nodefined : " nenÃ­ definovÃ¡n!",
			novalue : " je vyÅ¾adovÃ¡na nÃ¡vratovÃ¡ hodnota!",
			customarray : "Custom function mÄ›lÃ¡ vrÃ¡tit pole!",
			customfcheck : "Custom function by mÄ›la bÃ½t pÅ™Ã­tomna v pÅ™Ã­padÄ› custom checking!"
		}
	},
	view : {
	    caption: "Zobrazit zÃ¡znam",
	    bClose: "ZavÅ™Ã­t"
	},
	del : {
		caption: "Smazat",
		msg: "Smazat vybranÃ½(Ã©) zÃ¡znam(y)?",
		bSubmit: "Smazat",
		bCancel: "Storno"
	},
	nav : {
		edittext: " ",
		edittitle: "Editovat vybranÃ½ Å™Ã¡dek",
		addtext:" ",
		addtitle: "PÅ™idat novÃ½ Å™Ã¡dek",
		deltext: " ",
		deltitle: "Smazat vybranÃ½ zÃ¡znam ",
		searchtext: " ",
		searchtitle: "NajÃ­t zÃ¡znamy",
		refreshtext: "",
		refreshtitle: "Obnovit tabulku",
		alertcap: "VarovÃ¡nÃ­",
		alerttext: "ProsÃ­m, vyberte Å™Ã¡dek",
		viewtext: "",
		viewtitle: "Zobrazit vybranÃ½ Å™Ã¡dek"
	},
	col : {
		caption: "Zobrazit/SkrÃ½t sloupce",
		bSubmit: "UloÅ¾it",
		bCancel: "Storno"	
	},
	errors : {
		errcap : "Chyba",
		nourl : "NenÃ­ nastavena url",
		norecords: "Å½Ã¡dnÃ© zÃ¡znamy ke zpracovÃ¡nÃ­",
		model : "DÃ©lka colNames <> colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"Ne", "Po", "Ãšt", "St", "ÄŒt", "PÃ¡", "So",
				"NedÄ›le", "PondÄ›lÃ­", "ÃšterÃ½", "StÅ™eda", "ÄŒtvrtek", "PÃ¡tek", "Sobota"
			],
			monthNames: [
				"Led", "Ãšno", "BÅ™e", "Dub", "KvÄ›", "ÄŒer", "ÄŒvc", "Srp", "ZÃ¡Å™", "Å˜Ã­j", "Lis", "Pro",
				"Leden", "Ãšnor", "BÅ™ezen", "Duben", "KvÄ›ten", "ÄŒerven", "ÄŒervenec", "Srpen", "ZÃ¡Å™Ã­", "Å˜Ã­jen", "Listopad", "Prosinec"
			],
			AmPm : ["do","od","DO","OD"],
			S: function (j) {return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th'},
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
