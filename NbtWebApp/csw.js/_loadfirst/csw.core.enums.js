﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswEnums() {
    'use strict';
    
    var enums = (function () {

        var external = {
            constants: {unknownEnum: 'unknown'}
        };
        external.tryParse = function (cswEnum, enumMember, caseSensitive) {
            /// <summary>   Try to fetch an enum based on a string value. </summary>
            var ret = external.constants.unknownEnum;
            if (Csw.contains(cswEnum, enumMember)) {
                ret = cswEnum[enumMember];
            } else if (false === caseSensitive) {
                Csw.each(cswEnum, function (member) {
                    if (Csw.contains(cswEnum, member) &&
                        Csw.string(member).toLowerCase() === Csw.string(enumMember).toLowerCase()) {
                        ret = member;
                    }
                });
            }
            return ret;
        };
        external.editMode = {
            Edit: 'Edit',
            Add: 'Add',
            EditInPopup: 'EditInPopup',
            Demo: 'Demo',
            PrintReport: 'PrintReport',
            DefaultValue: 'DefaultValue',
            AuditHistoryInPopup: 'AuditHistoryInPopup',
            Preview: 'Preview',
            Table: 'Table'
        };
        external.errorType = {
            warning: {
                name: 'warning',
                cssclass: 'CswErrorMessage_Warning'
            },
            error: {
                name: 'error',
                cssclass: 'CswErrorMessage_Error'
            }
        };
        external.events = {
            CswNodeDelete: 'CswNodeDelete',
            ajax: {
                ajaxStart: 'ajaxStart',
                ajaxStop: 'ajaxStop',
                globalAjaxStart: 'globalAjaxStart',
                globalAjaxStop: 'globalAjaxStop'
            }
        };
        external.wizardSteps_InspectionDesign = {
            step1: {step: 1, description: 'Select an Inspection Target'},
            step2: {step: 2, description: 'Select an Inspection Design'},
            step3: {step: 3, description: 'Upload Template'},
            step4: {step: 4, description: 'Review Inspection Design'},
            step5: {step: 5, description: 'Finish'},
            stepcount: 5
        };
        external.wizardSteps_ScheduleRulesGrid = {
            step1: {step: 1, description: 'Select a Customer ID'},
            step2: {step: 2, description: 'Review the Scheduled Rules'},
            stepcount: 2
        };
        external.dialogButtons = {
            1: 'ok',
            2: 'ok/cancel',
            3: 'yes/no'
        };
        external.onObjectClassClick = {
            reauthenticate: 'reauthenticate',
            home: 'home',
            refresh: 'refresh',
            url: 'url'
        };
        external.inputTypes = {
            button: {id: 0, name: 'button', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: ''},
            checkbox: {id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: {required: true, allowed: true}, defaultwidth: ''},
            color: {id: 2, name: 'color', placeholder: false, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: ''},
            date: {id: 3, name: 'date', placeholder: false, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: '200px'},
            datetime: {id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: '200px'},
            'datetime-local': {id: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: '200px'},
            email: {id: 6, name: 'email', placeholder: true, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: '200px'},
            file: {id: 7, name: 'file', placeholder: false, autocomplete: false, value: {required: false, allowed: false}, defaultwidth: ''},
            hidden: {id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: ''},
            image: {id: 9, name: 'image', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: ''},
            month: {id: 10, name: 'month', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: ''},
            number: {id: 11, name: 'number', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: '200px'},
            password: {id: 12, name: 'password', placeholder: true, value: {required: false, allowed: true}, defaultwidth: '200px'},
            radio: {id: 13, name: 'radio', placeholder: false, autocomplete: false, value: {required: true, allowed: true}, defaultwidth: ''},
            range: {id: 14, name: 'range', placeholder: false, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: ''},
            reset: {id: 15, name: 'reset', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: ''},
            search: {id: 16, name: 'search', placeholder: true, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: ''},
            submit: {id: 17, name: 'submit', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: ''},
            tel: {id: 18, name: 'button', placeholder: true, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: ''},
            text: {id: 19, name: 'text', placeholder: true, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: '200px'},
            time: {id: 20, name: 'time', placeholder: false, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: '200px'},
            url: {id: 21, name: 'url', placeholder: true, autocomplete: true, value: {required: false, allowed: true}, defaultwidth: '200px'},
            week: {id: 22, name: 'week', placeholder: false, autocomplete: false, value: {required: false, allowed: true}, defaultwidth: ''}
        };
        external.viewMode = {
            grid: {name: 'Grid'},
            tree: {name: 'Tree'},
            list: {name: 'List'},
            table: {name: 'Table'}
        };
        external.rateIntervalTypes = {
            WeeklyByDay: 'WeeklyByDay',
            MonthlyByDate: 'MonthlyByDate',
            MonthlyByWeekAndDay: 'MonthlyByWeekAndDay',
            YearlyByDate: 'YearlyByDate'
        };
        external.multiEditDefaultValue = '[Unchanged]';

        external.imageButton_ButtonType = {
            None: -1,
            Add: 27,
            ArrowNorth: 28,
            ArrowEast: 29,
            ArrowSouth: 30,
            ArrowWest: 31,
            Calendar: 6,
            CheckboxFalse: 18,
            CheckboxNull: 19,
            CheckboxTrue: 20,
            Clear: 4,
            Clock: 10,
            ClockGrey: 11,
            Configure: 26,
            Delete: 4,
            Edit: 3,
            Fire: 5,
            PageFirst: 23,
            PagePrevious: 24,
            PageNext: 25,
            PageLast: 22,
            PinActive: 17,
            PinInactive: 15,
            Print: 2,
            Refresh: 9,
            SaveStatus: 13,
            Select: 32,
            ToggleActive: 1,
            ToggleInactive: 0,
            View: 8
        };

        external.searchCssClasses = {
            nodetype_select: {name: 'csw_search_nodetype_select'},
            property_select: {name: 'csw_search_property_select'}
        };

        external.appMode = {
            mode: 'full'
        };

        external.wizardSteps_ViewEditor = {
            viewselect: {step: 1, description: 'Choose a View', divId: 'step1_viewselect'},
            attributes: {step: 2, description: 'Edit View Attributes', divId: 'step2_attributes'},
            relationships: {step: 3, description: 'Add Relationships', divId: 'step3_relationships'},
            properties: {step: 4, description: 'Select Properties', divId: 'step4_properties'},
            filters: {step: 5, description: 'Set Filters', divId: 'step5_filters'},
            tuning: {step: 6, description: 'Fine Tuning', divId: 'step6_tuning'}
        };

        external.cssClasses_ViewBuilder = {
            subfield_select: {name: 'csw_viewbuilder_subfield_select'},
            filter_select: {name: 'csw_viewbuilder_filter_select'},
            default_filter: {name: 'csw_viewbuilder_default_filter'},
            filter_value: {name: 'csw_viewbuilder_filter_value'},
            metadatatype_static: {name: 'csw_viewbuilder_metadatatype_static'}
        };

        external.domElementEvent = {
            click: {name: 'click'},
            change: {name: 'change'},
            vclick: {name: 'vclick'},
            tap: {name: 'tap'}
        };

        external.objectClasses = {
            GenericClass: 'GenericClass',
            InspectionDesignClass: 'InspectionDesignClass'
        };

        external.nodeSpecies = {
            Plain: 'Plain',
            More: 'More'
        };

        external.subFieldNames = {
            Unknown: {name: 'unknown'},
            AllowedAnswers: {name: 'allowedanswers'},
            Answer: {name: 'answer'},
            Barcode: {name: 'barcode'},
            Blob: {name: 'blob'},
            Checked: {name: 'checked'},
            Column: {name: 'column'},
            Comments: {name: 'comments'},
            CompliantAnswers: {name: 'compliantanswers'},
            ContentType: {name: 'contenttype'},
            CorrectiveAction: {name: 'correctiveaction'},
            DateAnswered: {name: 'dateanswered'},
            DateCorrected: {name: 'datecorrected'},
            Href: {name: 'href'},
            Image: {name: 'image'},
            Interval: {name: 'interval'},
            IsCompliant: {name: 'iscompliant'},
            Mol: {name: 'mol'},
            Name: {name: 'name'},
            NodeID: {name: 'nodeid'},
            NodeType: {name: 'nodetype'},
            Number: {name: 'number'},
            Password: {name: 'password'},
            Path: {name: 'path'},
            Required: {name: 'required'},
            Row: {name: 'row'},
            Sequence: {name: 'sequence'},
            StartDateTime: {name: 'startdatetime'},
            Text: {name: 'text'},
            Units: {name: 'units'},
            Value: {name: 'value'},
            ViewID: {name: 'viewid'},
            ChangedDate: {name: 'changeddate'},
            Base: {name: 'base'},
            Exponent: {name: 'exponent'}
        };

        external.subFieldsMap = {
            AuditHistoryGrid: {name: 'AuditHistoryGrid', subfields: {}},
            Barcode: {
                name: 'Barcode',
                subfields: {
                    Barcode: external.subFieldNames.Barcode,
                    Sequence: external.subFieldNames.Number
                }
            },
            Button: {
                name: 'Button',
                subfields: {
                    Text: external.subFieldNames.Text
                }
            },
            Composite: {name: 'Composite', subfields: {}},
            DateTime: {
                name: 'DateTime',
                subfields: {
                    Value: {
                        Date: {name: 'date'},
                        Time: {name: 'time'},
                        DateFormat: {name: 'dateformat'},
                        TimeFormat: {name: 'timeformat'}
                    },
                    DisplayMode: {
                        Date: {name: 'Date'},
                        Time: {name: 'Time'},
                        DateTime: {name: 'DateTime'}
                    }
                }
            },
            File: {name: 'File', subfields: {}},
            Grid: {name: 'Grid', subfields: {}},
            Image: {name: 'Image', subfields: {}},
            Link: {
                name: 'Link',
                subfields: {
                    Text: external.subFieldNames.Text,
                    Href: external.subFieldNames.Href
                }
            },
            List: {
                name: 'List',
                subfields: {
                    Value: external.subFieldNames.Value
                }
            },
            Location: {name: 'Location', subfields: {}},
            LocationContents: {name: 'LocationContents', subfields: {}},
            Logical: {
                name: 'Logical',
                subfields: {
                    Checked: external.subFieldNames.Checked
                }
            },
            LogicalSet: {name: 'LogicalSet', subfields: {}},
            Memo: {
                name: 'Memo',
                subfields: {
                    Text: external.subFieldNames.Text
                }
            },
            MTBF: {name: 'MTBF', subfields: {}},
            MultiList: {name: 'MultiList', subfields: {}},
            NodeTypeSelect: {name: 'NodeTypeSelect', subfields: {}},
            Number: {
                name: 'Number',
                subfields: {
                    Value: external.subFieldNames.Value
                }
            },
            Password: {
                name: 'Password',
                subfields: {
                    Password: external.subFieldNames.Password,
                    ChangedDate: external.subFieldNames.ChangedDate
                }
            },
            PropertyReference: {name: 'PropertyReference', subfields: {}},
            Quantity: {
                name: 'Quantity',
                subfields: {
                    Value: external.subFieldNames.Value,
                    Units: external.subFieldNames.Number
                }
            },
            Question: {
                name: 'Question',
                subfields: {
                    Answer: external.subFieldNames.Answer,
                    CorrectiveAction: external.subFieldNames.CorrectiveAction,
                    IsCompliant: external.subFieldNames.IsCompliant,
                    Comments: external.subFieldNames.Comments,
                    DateAnswered: external.subFieldNames.DateAnswered,
                    DateCorrected: external.subFieldNames.DateCorrected
                }
            },
            Relationship: {name: 'Relationship', subfields: {}},
            Scientific: {name: 'Scientific', subfields: {}},
            Sequence: {name: 'Sequence', subfields: {}},
            Static: {
                name: 'Static',
                subfields: {
                    Text: external.subFieldNames.Text
                }
            },
            Text: {
                name: 'Text',
                subfields: {
                    Text: external.subFieldNames.Text
                }
            },
            TimeInterval: {name: 'TimeInterval', subfields: {}},
            UserSelect: {name: 'UserSelect', subfields: {}},
            ViewPickList: {name: 'ViewPickList', subfields: {}},
            ViewReference: {name: 'ViewReference', subfields: {}}
        };

        external.cssClasses_ViewEdit = {
            vieweditor_viewrootlink: {name: 'vieweditor_viewrootlink'},
            vieweditor_viewrellink: {name: 'vieweditor_viewrellink'},
            vieweditor_viewproplink: {name: 'vieweditor_viewproplink'},
            vieweditor_viewfilterlink: {name: 'vieweditor_viewfilterlink'},
            vieweditor_addfilter: {name: 'vieweditor_addfilter'},
            vieweditor_deletespan: {name: 'vieweditor_deletespan'},
            vieweditor_childselect: {name: 'vieweditor_childselect'}
        };

        external.viewChildPropNames = {
            root: {name: 'root'},
            childrelationships: {name: 'childrelationships'},
            properties: {name: 'properties'},
            filters: {name: 'filters'},
            propfilters: {name: 'filters'},
            filtermodes: {name: 'filtermodes'}
        };

        external.nodeTree_DefaultSelect = {
            root: {name: 'root'},
            firstchild: {name: 'firstchild'},
            none: {name: 'none'}
        };

        return external;
    }());
    Csw.register('enum', enums);
    Csw.enums = Csw.enums || enums;
}());