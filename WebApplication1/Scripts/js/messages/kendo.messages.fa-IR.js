kendo.culture("fa-IR");
kendo.ui.Locale = "Persian (fa-IR)";

!function (e) {
    "function" == typeof define && define.amd ? define(["kendo.core.min"], e) : e()
}

(function () {
    !function (e, t) {
        kendo.ui.FlatColorPicker && (kendo.ui.FlatColorPicker.prototype.options.messages = e.extend(!0, kendo.ui.FlatColorPicker.prototype.options.messages, {
            apply: "Apply",
            cancel: "Cancel"
        })),
        kendo.ui.ColorPicker && (kendo.ui.ColorPicker.prototype.options.messages = e.extend(!0, kendo.ui.ColorPicker.prototype.options.messages, {
            apply: "Apply",
            cancel: "Cancel"
        })),
        kendo.ui.ColumnMenu && (kendo.ui.ColumnMenu.prototype.options.messages = e.extend(!0, kendo.ui.ColumnMenu.prototype.options.messages, {
            sortAscending: "مرتب سازی صعودی",
            sortDescending: "مرتب سازی نزولی",
            filter: "فیلتر",
            columns: "ستون ها",
            lock: "قفل کردن",
            done: "تمام",
            unlock: "بارکردن قفل",
            settings: "تنظیمات"
        })),
        kendo.ui.Editor && (kendo.ui.Editor.prototype.options.messages = e.extend(!0, kendo.ui.Editor.prototype.options.messages, {
            bold: "Bold",
            italic: "Italic",
            underline: "Underline",
            strikethrough: "Strikethrough",
            superscript: "Superscript",
            subscript: "Subscript",
            justifyCenter: "Center text",
            justifyLeft: "Align text left",
            justifyRight: "Align text right",
            justifyFull: "Justify",
            insertUnorderedList: "Insert unordered list",
            insertOrderedList: "Insert ordered list",
            indent: "Indent",
            outdent: "Outdent",
            createLink: "Insert hyperlink",
            unlink: "Remove hyperlink",
            insertImage: "Insert image",
            insertFile: "Insert file",
            insertHtml: "Insert HTML",
            viewHtml: "View HTML",
            fontName: "Select font family",
            fontNameInherit: "(inherited font)",
            fontSize: "Select font size",
            fontSizeInherit: "(inherited size)",
            formatBlock: "Format",
            formatting: "Format",
            foreColor: "Color",
            backColor: "Background color",
            style: "Styles",
            emptyFolder: "Empty Folder",
            uploadFile: "Upload",
            orderBy: "Arrange by:",
            orderBySize: "Size",
            orderByName: "Name",
            invalidFileType: 'The selected file "{0}" is not valid. Supported file types are {1}.',
            deleteFile: 'Are you sure you want to delete "{0}"?',
            overwriteFile: 'A file with name "{0}" already exists in the current directory. Do you want to overwrite it?',
            directoryNotFound: "A directory with this name was not found.",
            imageWebAddress: "Web address",
            imageAltText: "Alternate text",
            imageWidth: "Width (px)",
            imageHeight: "Height (px)",
            fileWebAddress: "Web address",
            fileTitle: "Title",
            linkWebAddress: "Web address",
            linkText: "Text",
            linkToolTip: "ToolTip",
            linkOpenInNewWindow: "Open link in new window",
            dialogUpdate: "Update",
            dialogInsert: "Insert",
            dialogButtonSeparator: "or",
            dialogCancel: "Cancel",
            createTable: "Create table",
            addColumnLeft: "Add column on the left",
            addColumnRight: "Add column on the right",
            addRowAbove: "Add row above",
            addRowBelow: "Add row below",
            deleteRow: "Delete row",
            deleteColumn: "Delete column"
        })),
        kendo.ui.FileBrowser && (kendo.ui.FileBrowser.prototype.options.messages = e.extend(!0, kendo.ui.FileBrowser.prototype.options.messages, {
            uploadFile: "Upload",
            orderBy: "Arrange by",
            orderByName: "Name",
            orderBySize: "Size",
            directoryNotFound: "A directory with this name was not found.",
            emptyFolder: "Empty Folder",
            deleteFile: 'Are you sure you want to delete "{0}"?', invalidFileType: 'The selected file "{0}" is not valid. Supported file types are {1}.', overwriteFile: 'A file with name "{0}" already exists in the current directory. Do you want to overwrite it?', dropFilesHere: "drop file here to upload",
            search: "Search"
        })),
        kendo.ui.FilterCell && (kendo.ui.FilterCell.prototype.options.messages = e.extend(!0, kendo.ui.FilterCell.prototype.options.messages, {
            isTrue: "درست باشد",
            isFalse: "نادرست باشد",
            filter: "فیلتر",
            clear: "پاک کردن",
            operator: "عملگر"
        })),
        kendo.ui.FilterCell && (kendo.ui.FilterCell.prototype.options.operators = e.extend(!0, kendo.ui.FilterCell.prototype.options.operators, {
            string: {
                eq: "برابر",
                neq: "مخالف",
                startswith: "شروع می شوند",
                contains: "دارا می باشند",
                doesnotcontain: "دارا نمی باشند",
                endswith: "خاتمه می یابند"
            },
            number: {
                eq: "مساوی",
                neq: "مخالف",
                gte: "بزرگتر یا مساوی",
                gt: "بزرگتر",
                lte: "کوچکتر یا مساوی",
                lt: "کوچکتر"
            },
            date: {
                eq: "مساوی",
                neq: "مخالف",
                gte: "بزرگتر یا مساوی",
                gt: "بزرگتر",
                lte: "کوچکتر یا مساوی",
                lt: "کوچکتر"
            },
            enums: {
                eq: "برابر",
                neq: "مخالف"
            }
        })),
        kendo.ui.FilterMenu && (kendo.ui.FilterMenu.prototype.options.messages = e.extend(!0, kendo.ui.FilterMenu.prototype.options.messages, {
            info: "نشان دادن مواردی که:",        // sets the text on top of the filter menu
            filter: "فیلتر",      // sets the text for the "Filter" button
            clear: "پاک کردن",        // sets the text for the "Clear" button
            // when filtering boolean numbers
            isTrue: "درست باشد", // sets the text for "isTrue" radio button
            isFalse: "نادرست باشد",     // sets the text for "isFalse" radio button
            //changes the text of the "And" and "Or" of the filter menu
            and: "و",
            or: "یا",
            selectValue: "-انتخاب کنید-",
            operator: "عملگر",
            value: "مقدار",
            cancel: "انصراف"
        })),
        kendo.ui.FilterMenu && (kendo.ui.FilterMenu.prototype.options.operators = e.extend(0, kendo.ui.FilterMenu.prototype.options.operators, {
            string: {
                contains: "شامل می شود",
                startswith: "شروع می شود",
                //doesnotcontain: "دارا نمی باشند",
                endswith: "خاتمه می یابد",
                //isnull: "تهی باشد",
                //isnotnull: "تهی نباشد",
                eq: "برابر",
                //neq: "مخالف",
                //isempty: "بی مقدار",
                //isnotempty: "دارای مقدار"
            },
            number: {
                eq: "مساوی",
                neq: "مخالف",
                gte: "بزرگتر یا مساوی",
                gt: "بزرگتر",
                lte: "کوچکتر یا مساوی",
                lt: "کوچکتر",
                isnull: "تهی باشد",
                isnotnull: "تهی نباشد"
            },
            date: {
                eq: "مساوی",
                neq: "مخالف",
                gte: "بزرگتر یا مساوی",
                gt: "بزرگتر",
                lte: "کوچکتر یا مساوی",
                lt: "کوچکتر",
                isnull: "تهی باشد",
                isnotnull: "تهی نباشد"
            },
            enums: {
                eq: "مساوی",
                neq: "مخالف",
                //isnull: "تهی باشد",
                //isnotnull: "تهی نباشد"
            }
        })),
        kendo.ui.FilterMultiCheck && (kendo.ui.FilterMultiCheck.prototype.options.messages = e.extend(!0, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            checkAll: "Select All",
            clear: "Clear",
            filter: "Filter"
        })),
        kendo.ui.Gantt && (kendo.ui.Gantt.prototype.options.messages = e.extend(!0, kendo.ui.Gantt.prototype.options.messages, {
            actions: {
                addChild: "Add Child",
                append: "Add Task",
                insertAfter: "Add Below",
                insertBefore: "Add Above",
                pdf: "Export to PDF"
            },
            cancel: "Cancel",
            deleteDependencyWindowTitle: "Delete dependency",
            deleteTaskWindowTitle: "Delete task",
            destroy: "Delete",
            editor: {
                assingButton: "Assign",
                editorTitle: "Task",
                end: "End",
                percentComplete: "Complete",
                resources: "Resources",
                resourcesEditorTitle: "Resources",
                resourcesHeader: "Resources",
                start: "Start",
                title: "Title",
                unitsHeader: "Units"
            },
            save: "Save",
            views: {
                day: "Day",
                end: "End",
                month: "Month",
                start: "Start",
                week: "Week",
                year: "Year"
            }
        })),
        kendo.ui.Grid && (kendo.ui.Grid.prototype.options.messages = e.extend(!0, kendo.ui.Grid.prototype.options.messages, {
            commands: {
                cancel: "لغو تغییرات",
                canceledit: "انصراف",
                create: "ایجاد رکورد جدید",
                destroy: "حذف",
                edit: "ویرایش",
                excel: "خروجی اکسل",
                pdf: "خروجی PDF",
                save: "ذخیره تغییرات",
                select: "انتخاب",
                update: "ثبت"
            },
            editable: {
                cancelDelete: "انصراف",
                confirmation: "آیا از حذف این رکورد مطمئن هستید؟",
                confirmDelete: "حذف"
            },
            noRecords: "هیچ موردی یافت نشد"
        })),
        kendo.ui.Groupable && (kendo.ui.Groupable.prototype.options.messages = e.extend(!0, kendo.ui.Groupable.prototype.options.messages, {
            empty: "جهت گروه کردن لطفا هدر ستون را در این قسمت درگ و دراپ نمایید."
        })),
        kendo.ui.NumericTextBox && (kendo.ui.NumericTextBox.prototype.options = e.extend(!0, kendo.ui.NumericTextBox.prototype.options, {
            upArrowText: "Increase value",
            downArrowText: "Decrease value"
        })),
        kendo.ui.Pager && (kendo.ui.Pager.prototype.options.messages = e.extend(!0, kendo.ui.Pager.prototype.options.messages, {
            display: "{0} تا {1} از {2} مورد",
            empty: "موردی یافت نشد",
            page: "صفحه",
            of: "از {0}",
            itemsPerPage: "تعداد موارد در هر صفحه",
            first: "اولین",
            previous: "قبلی",
            next: "بعدی",
            last: "آخرین",
            refresh: "بارگذاری مجدد",
            morePages: "صفحات بیشتر",
            allPages: "همه صفحات"
        })),
        kendo.ui.PivotGrid && (kendo.ui.PivotGrid.prototype.options.messages = e.extend(!0, kendo.ui.PivotGrid.prototype.options.messages, {
            measureFields: "Drop Data Fields Here",
            columnFields: "Drop Column Fields Here",
            rowFields: "Drop Rows Fields Here"
        })),
        kendo.ui.PivotFieldMenu && (kendo.ui.PivotFieldMenu.prototype.options.messages = e.extend(!0, kendo.ui.PivotFieldMenu.prototype.options.messages, {
            info: "Show items with value that:",
            filterFields: "Fields Filter",
            filter: "Filter",
            include: "Include Fields...",
            title: "Fields to include",
            clear: "Clear",
            ok: "Ok",
            cancel: "Cancel",
            operators: {
                contains: "Contains",
                doesnotcontain: "Does not contain",
                startswith: "Starts with",
                endswith: "Ends with",
                eq: "Is equal to",
                neq: "Is not equal to"
            }
        })),
        kendo.ui.RecurrenceEditor && (kendo.ui.RecurrenceEditor.prototype.options.messages = e.extend(!0, kendo.ui.RecurrenceEditor.prototype.options.messages, {
            frequencies: {
                never: "Never",
                hourly: "Hourly",
                daily: "Daily",
                weekly: "Weekly",
                monthly: "Monthly",
                yearly: "Yearly"
            },
            hourly: {
                repeatEvery: "Repeat every: ",
                interval: " hour(s)"
            },
            daily: {
                repeatEvery: "Repeat every: ",
                interval: " day(s)"
            },
            weekly: {
                interval: " week(s)",
                repeatEvery: "Repeat every: ",
                repeatOn: "Repeat on: "
            },
            monthly: {
                repeatEvery: "Repeat every: ",
                repeatOn: "Repeat on: ",
                interval: " month(s)",
                day: "Day "
            },
            yearly: {
                repeatEvery: "Repeat every: ",
                repeatOn: "Repeat on: ",
                interval: " year(s)",
                of: " of "
            },
            end: {
                label: "End:",
                mobileLabel: "Ends",
                never: "Never",
                after: "After ",
                occurrence: " occurrence(s)",
                on: "On "
            },
            offsetPositions: {
                first: "first",
                second: "second",
                third: "third",
                fourth: "fourth",
                last: "last"
            },
            weekdays: {
                day: "day",
                weekday: "weekday",
                weekend: "weekend day"
            }
        })),
        kendo.ui.Scheduler && (kendo.ui.Scheduler.prototype.options.messages = e.extend(!0, kendo.ui.Scheduler.prototype.options.messages, {
            allDay: "all day",
            date: "Date",
            event: "Event",
            time: "Time",
            showFullDay: "Show full day",
            showWorkDay: "Show business hours",
            today: "Today",
            save: "Save",
            cancel: "Cancel",
            destroy: "Delete",
            deleteWindowTitle: "Delete event",
            ariaSlotLabel: "Selected from {0:t} to {1:t}",
            ariaEventLabel: "{0} on {1:D} at {2:t}",
            editable: {
                confirmation: "Are you sure you want to delete this event?"
            },
            views: {
                day: "Day",
                week: "Week",
                workWeek: "Work Week",
                agenda: "Agenda",
                month: "Month"
            },
            recurrenceMessages: {
                deleteWindowTitle: "Delete Recurring Item",
                deleteWindowOccurrence: "Delete current occurrence",
                deleteWindowSeries: "Delete the series",
                editWindowTitle: "Edit Recurring Item",
                editWindowOccurrence: "Edit current occurrence",
                editWindowSeries: "Edit the series",
                deleteRecurring: "Do you want to delete only this event occurrence or the whole series?",
                editRecurring: "Do you want to edit only this event occurrence or the whole series?"
            },
            editor: {
                title: "Title",
                start: "Start",
                end: "End",
                allDayEvent: "All day event",
                description: "Description",
                repeat: "Repeat",
                timezone: " ",
                startTimezone: "Start timezone",
                endTimezone: "End timezone",
                separateTimezones: "Use separate start and end time zones",
                timezoneEditorTitle: "Timezones",
                timezoneEditorButton: "Time zone",
                timezoneTitle: "Time zones",
                noTimezone: "No timezone",
                editorTitle: "Event"
            }
        })),
        kendo.spreadsheet && kendo.spreadsheet.messages.borderPalette && (kendo.spreadsheet.messages.borderPalette = e.extend(!0, kendo.spreadsheet.messages.borderPalette, {
            allBorders: "All borders",
            insideBorders: "Inside borders",
            insideHorizontalBorders: "Inside horizontal borders",
            insideVerticalBorders: "Inside vertical borders",
            outsideBorders: "Outside borders",
            leftBorder: "Left border",
            topBorder: "Top border",
            rightBorder: "Right border",
            bottomBorder: "Bottom border",
            noBorders: "No border",
            reset: "Reset color",
            customColor: "Custom color...",
            apply: "Apply",
            cancel: "Cancel"
        })),
        kendo.spreadsheet && kendo.spreadsheet.messages.dialogs && (kendo.spreadsheet.messages.dialogs = e.extend(!0, kendo.spreadsheet.messages.dialogs, {
            apply: "Apply",
            save: "Save",
            cancel: "Cancel",
            remove: "Remove",
            okText: "OK",
            formatCellsDialog: {
                title: "Format",
                categories: {
                    number: "Number",
                    currency: "Currency",
                    date: "Date"
                }
            },
            fontFamilyDialog: {
                title: "Font"
            },
            fontSizeDialog: {
                title: "Font size"
            },
            bordersDialog: {
                title: "Borders"
            },
            alignmentDialog: {
                title: "Alignment",
                buttons: {
                    justtifyLeft: "Align left",
                    justifyCenter: "Center",
                    justifyRight: "Align right",
                    justifyFull: "Justify",
                    alignTop: "Align top",
                    alignMiddle: "Align middle",
                    alignBottom: "Align bottom"
                }
            },
            mergeDialog: {
                title: "Merge cells",
                buttons: {
                    mergeCells: "Merge all",
                    mergeHorizontally: "Merge horizontally",
                    mergeVertically: "Merge vertically",
                    unmerge: "Unmerge"
                }
            },
            freezeDialog: {
                title: "Freeze panes",
                buttons: {
                    freezePanes: "Freeze panes",
                    freezeRows: "Freeze rows",
                    freezeColumns: "Freeze columns",
                    unfreeze: "Unfreeze panes"
                }
            },
            validationDialog: {
                title: "Data Validation",
                hintMessage: "Please enter a valid {0} value {1}.",
                hintTitle: "Validation {0}",
                criteria: {
                    any: "Any value",
                    number: "Number",
                    text: "Text",
                    date: "Date",
                    custom: "Custom Formula",
                    list: "List"
                },
                comparers: {
                    greaterThan: "greater than",
                    lessThan: "less than",
                    between: "between",
                    notBetween: "not between",
                    equalTo: "equal to",
                    notEqualTo: "not equal to",
                    greaterThanOrEqualTo: "greater than or equal to",
                    lessThanOrEqualTo: "less than or equal to"
                },
                comparerMessages: {
                    greaterThan: "greater than {0}",
                    lessThan: "less than {0}",
                    between: "between {0} and {1}",
                    notBetween: "not between {0} and {1}",
                    equalTo: "equal to {0}",
                    notEqualTo: "not equal to {0}",
                    greaterThanOrEqualTo: "greater than or equal to {0}",
                    lessThanOrEqualTo: "less than or equal to {0}",
                    custom: "that satisfies the formula: {0}"
                },
                labels: {
                    criteria: "Criteria",
                    comparer: "Comparer",
                    min: "Min",
                    max: "Max",
                    value: "Value",
                    start: "Start",
                    end: "End",
                    onInvalidData: "On invalid data",
                    rejectInput: "Reject input",
                    showWarning: "Show warning",
                    showHint: "Show hint",
                    hintTitle: "Hint title",
                    hintMessage: "Hint message",
                    ignoreBlank: "Ignore blank"
                },
                placeholders: {
                    typeTitle: "Type title",
                    typeMessage: "Type message"
                }
            },
            saveAsDialog: {
                title: "Save As...",
                labels: {
                    fileName: "File name",
                    saveAsType: "Save as type"
                }
            },
            exportAsDialog: {
                title: "Export...",
                labels: {
                    fileName: "File name",
                    saveAsType: "Save as type",
                    exportArea: "Export",
                    paperSize: "Paper size",
                    margins: "Margins",
                    orientation: "Orientation",
                    print: "Print",
                    guidelines: "Guidelines",
                    center: "Center",
                    horizontally: "Horizontally",
                    vertically: "Vertically"
                }
            },
            modifyMergedDialog: {
                errorMessage: "Cannot change part of a merged cell."
            },
            useKeyboardDialog: {
                title: "Copying and pasting",
                errorMessage: "These actions cannot be invoked through the menu. Please use the keyboard shortcuts instead:",
                labels: {
                    forCopy: "for copy",
                    forCut: "for cut",
                    forPaste: "for paste"
                }
            },
            unsupportedSelectionDialog: {
                errorMessage: "That action cannot be performed on multiple selection."
            }
        })),
        kendo.spreadsheet && kendo.spreadsheet.messages.filterMenu && (kendo.spreadsheet.messages.filterMenu = e.extend(!0, kendo.spreadsheet.messages.filterMenu, {
            sortAscending: "Sort range A to Z",
            sortDescending: "Sort range Z to A",
            filterByValue: "Filter by value",
            filterByCondition: "Filter by condition",
            apply: "Apply",
            search: "Search",
            addToCurrent: "Add to current selection",
            clear: "Clear",
            blanks: "(Blanks)",
            operatorNone: "None",
            and: "AND",
            or: "OR",
            operators: {
                string: {
                    contains: "Text contains",
                    doesnotcontain: "Text does not contain",
                    startswith: "Text starts with",
                    endswith: "Text ends with"
                },
                date: {
                    eq: "Date is",
                    neq: "Date is not",
                    lt: "Date is before",
                    gt: "Date is after"
                },
                number: {
                    eq: "Is equal to",
                    neq: "Is not equal to",
                    gte: "Is greater than or equal to",
                    gt: "Is greater than",
                    lte: "Is less than or equal to",
                    lt: "Is less than"
                }
            }
        })),
        kendo.spreadsheet && kendo.spreadsheet.messages.toolbar && (kendo.spreadsheet.messages.toolbar = e.extend(!0, kendo.spreadsheet.messages.toolbar, {
            addColumnLeft: "Add column left",
            addColumnRight: "Add column right",
            addRowAbove: "Add row above",
            addRowBelow: "Add row below",
            alignment: "Alignment",
            alignmentButtons: {
                justtifyLeft: "Align left",
                justifyCenter: "Center",
                justifyRight: "Align right",
                justifyFull: "Justify",
                alignTop: "Align top",
                alignMiddle: "Align middle",
                alignBottom: "Align bottom"
            },
            backgroundColor: "Background",
            bold: "Bold",
            borders: "Borders",
            colorPicker: {
                reset: "Reset color",
                customColor: "Custom color..."
            },
            copy: "Copy",
            cut: "Cut",
            deleteColumn: "Delete column",
            deleteRow: "Delete row",
            excelImport: "Import from Excel...",
            filter: "Filter",
            fontFamily: "Font",
            fontSize: "Font size",
            format: "Custom format...",
            formatTypes: {
                automatic: "Automatic",
                number: "Number",
                percent: "Percent",
                financial: "Financial",
                currency: "Currency",
                date: "Date",
                time: "Time",
                dateTime: "Date time",
                duration: "Duration",
                moreFormats: "More formats..."
            },
            formatDecreaseDecimal: "Decrease decimal",
            formatIncreaseDecimal: "Increase decimal",
            freeze: "Freeze panes",
            freezeButtons: {
                freezePanes: "Freeze panes",
                freezeRows: "Freeze rows",
                freezeColumns: "Freeze columns",
                unfreeze: "Unfreeze panes"
            },
            italic: "Italic",
            merge: "Merge cells",
            mergeButtons: {
                mergeCells: "Merge all",
                mergeHorizontally: "Merge horizontally",
                mergeVertically: "Merge vertically",
                unmerge: "Unmerge"
            },
            open: "Open...",
            paste: "Paste",
            quickAccess: {
                redo: "Redo",
                undo: "Undo"
            },
            saveAs: "Save As...",
            sortAsc: "Sort ascending",
            sortDesc: "Sort descending",
            sortButtons: {
                sortSheetAsc: "Sort sheet A to Z",
                sortSheetDesc: "Sort sheet Z to A",
                sortRangeAsc: "Sort range A to Z",
                sortRangeDesc: "Sort range Z to A"
            },
            textColor: "Text Color",
            textWrap: "Wrap text",
            underline: "Underline",
            validation: "Data validation..."
        })),
        kendo.spreadsheet && kendo.spreadsheet.messages.view && (kendo.spreadsheet.messages.view = e.extend(!0, kendo.spreadsheet.messages.view, {
            errors: {
                shiftingNonblankCells: "Cannot insert cells due to data loss possibility. Select another insert location or delete the data from the end of your worksheet.",
                filterRangeContainingMerges: "Cannot create a filter within a range containing merges",
                validationError: "The value that you entered violates the validation rules set on the cell."
            },
            tabs: {
                home: "Home",
                insert: "Insert",
                data: "Data"
            }
        })),
        kendo.ui.Slider && (kendo.ui.Slider.prototype.options = e.extend(!0, kendo.ui.Slider.prototype.options, {
            increaseButtonTitle: "Increase",
            decreaseButtonTitle: "Decrease"
        })),
        kendo.ui.TreeList && (kendo.ui.TreeList.prototype.options.messages = e.extend(!0, kendo.ui.TreeList.prototype.options.messages, {
            noRows: "هیچ موردی یافت نشد",
            loading: "در حال بارگذاری...",
            requestFailed: "بروز خطا!",
            retry: "تلاش مجدد",
            commands: {
                canceledit: "انصراف",
                create: "ایجاد رکورد جدید",
                destroy: "حذف",
                edit: "ویرایش",
                excel: "خروجی اکسل",
                pdf: "خروجی PDF",
                update: "به روزرسانی",
                createchild: "اضافه"
            }
        })),
        kendo.ui.TreeList && (kendo.ui.TreeList.prototype.options.columnMenu = e.extend(!0, kendo.ui.TreeList.prototype.options.columnMenu, {
            messages: {
                sortAscending: "مرتب سازی صعودی",
                sortDescending: "مرتب سازی نزولی",
                filter: "فیلتر",
                columns: "ستون ها"
            }
        })),
        kendo.ui.TreeView && (kendo.ui.TreeView.prototype.options.messages = e.extend(!0, kendo.ui.TreeView.prototype.options.messages, {
            loading: "Loading...",
            requestFailed: "Request failed.",
            retry: "Retry"
        })),
        kendo.ui.Upload && (kendo.ui.Upload.prototype.options.localization = e.extend(!0, kendo.ui.Upload.prototype.options.localization, {
            select: "Select files...",
            cancel: "Cancel",
            retry: "Retry",
            remove: "Remove",
            uploadSelectedFiles: "Upload files",
            dropFilesHere: "drop files here to upload",
            statusUploading: "uploading",
            statusUploaded: "uploaded",
            statusWarning: "warning",
            statusFailed: "failed",
            headerStatusUploading: "Uploading...",
            headerStatusUploaded: "Done"
        })),
        kendo.ui.Validator && (kendo.ui.Validator.prototype.options.messages = e.extend(!0, kendo.ui.Validator.prototype.options.messages, {
            required: "وارد نمودن {0} الزامی است.",
            pattern: "{0} را صحیح وارد نمائید.",
            min: "{0} باید بزرگتر از {1} باشد",
            max: "{0} باید کوچکتر از {1} باشد",
            step: "{0} صحیح نمی باشد.",
            email: "{0} به عنوان آدرس ایمیل صحیح وارد نشده است.",
            url: "{0} به عنوان آدرس اینترنتی صحیح وارد نشده است",
            date: "{0} به عنوان تاریخ صحیح وارد نشده است",
            dateCompare: "تاریخ پایان میبایست بزرگتر یا مساوی تاریخ شروع باشد"
        }))
    }(window.kendo.jQuery)
});