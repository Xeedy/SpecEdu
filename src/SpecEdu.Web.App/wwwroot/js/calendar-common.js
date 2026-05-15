/**
 * SpecEdu Calendar — shared initialisation for all calendar pages.
 *
 * Usage:
 *   SpecEduCalendar.init({
 *       elementId:      'calendar',
 *       eventsUrl:      '/SchoolAdmin/Calendar?handler=Events',
 *       locale:         'cs',
 *       labels:         { today:'Dnes', month:'Měsíc', week:'Týden', day:'Den', list:'Seznam' },
 *       isAdmin:        true,
 *       detailsBaseUrl: '/SchoolAdmin/Calendar/Details',   // optional
 *       createUrl:      '/SchoolAdmin/Calendar/Create',    // optional (admin only)
 *       editBaseUrl:    '/SchoolAdmin/Calendar/Edit'       // optional (admin only)
 *   });
 */
var SpecEduCalendar = (function () {
    'use strict';

    function isMobile() {
        return window.innerWidth < 768;
    }

    function desktopToolbar() {
        return {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        };
    }

    function mobileToolbar() {
        return {
            left: 'prev,next',
            center: 'title',
            right: 'listWeek,dayGridMonth'
        };
    }

    function formatDate(dt) {
        if (!dt) return '';
        return dt.toLocaleDateString('cs-CZ', { day: '2-digit', month: '2-digit', year: 'numeric' });
    }

    function formatTime(dt) {
        if (!dt) return '';
        return dt.toLocaleTimeString('cs-CZ', { hour: '2-digit', minute: '2-digit' });
    }

    function getTypeBadgeClass(type) {
        var map = {
            0: 'bg-primary',      // ParentTeacherMeeting
            1: 'bg-success',      // IndividualConsultation
            2: 'bg-danger',       // PlppReview
            3: 'bg-danger',       // IvpReview
            4: 'text-bg-purple',  // CounselorMeeting
            5: 'bg-warning text-dark', // ExternalSpecialistMeeting
            6: 'bg-info',         // SchoolEvent
            7: 'bg-secondary'     // Other
        };
        return map[type] || 'bg-secondary';
    }

    function getStatusBadgeClass(status) {
        var map = {
            0: 'bg-warning text-dark', // Scheduled
            1: 'bg-success',           // Confirmed
            2: 'bg-secondary',         // Completed
            3: 'bg-danger',            // Cancelled
            4: 'bg-info'               // Rescheduled
        };
        return map[status] || 'bg-primary';
    }

    function init(config) {
        var el = document.getElementById(config.elementId || 'calendar');
        if (!el) return;

        var calendar = new FullCalendar.Calendar(el, {
            initialView: isMobile() ? 'listWeek' : 'dayGridMonth',
            locale: config.locale || 'cs',
            headerToolbar: isMobile() ? mobileToolbar() : desktopToolbar(),
            buttonText: {
                today: config.labels.today || 'Today',
                month: config.labels.month || 'Month',
                week:  config.labels.week  || 'Week',
                day:   config.labels.day   || 'Day',
                list:  config.labels.list  || 'List'
            },
            height: 'auto',
            navLinks: true,
            editable: false,

            events: function (info, successCallback, failureCallback) {
                var url = config.eventsUrl +
                    (config.eventsUrl.indexOf('?') > -1 ? '&' : '?') +
                    'start=' + info.startStr + '&end=' + info.endStr;
                fetch(url)
                    .then(function (r) { return r.json(); })
                    .then(function (data) {
                        var mapped = data.map(function (evt) {
                            return {
                                id: evt.id,
                                title: evt.title,
                                start: evt.start,
                                end: evt.end,
                                backgroundColor: evt.color,
                                borderColor: evt.borderColor,
                                extendedProps: {
                                    studentName: evt.studentName,
                                    location: evt.location,
                                    type: evt.type,
                                    status: evt.status,
                                    studentId: evt.studentId
                                }
                            };
                        });
                        successCallback(mapped);
                    })
                    .catch(function () { failureCallback(); });
            },

            eventClick: function (info) {
                info.jsEvent.preventDefault();
                var evt = info.event;
                var props = evt.extendedProps;

                // Modal elements
                var modal = document.getElementById('calendarEventModal');
                if (!modal) return;

                // Title + color dot
                var titleEl = document.getElementById('eventModalTitle');
                var dotEl = document.getElementById('eventModalColorDot');
                if (titleEl) titleEl.textContent = evt.title;
                if (dotEl) dotEl.style.backgroundColor = evt.backgroundColor || '#6c757d';

                // Date
                var dateEl = document.getElementById('eventModalDate');
                if (dateEl) dateEl.textContent = formatDate(evt.start);

                // Time
                var timeEl = document.getElementById('eventModalTime');
                if (timeEl) {
                    var timeTxt = formatTime(evt.start);
                    if (evt.end) timeTxt += ' \u2013 ' + formatTime(evt.end);
                    timeEl.textContent = timeTxt;
                }

                // Student
                var studentRow = document.getElementById('eventModalStudentRow');
                var studentEl = document.getElementById('eventModalStudent');
                if (studentRow && studentEl) {
                    if (props.studentName) {
                        studentEl.textContent = props.studentName;
                        studentRow.style.display = '';
                    } else {
                        studentRow.style.display = 'none';
                    }
                }

                // Location
                var locationRow = document.getElementById('eventModalLocationRow');
                var locationEl = document.getElementById('eventModalLocation');
                if (locationRow && locationEl) {
                    if (props.location) {
                        locationEl.textContent = props.location;
                        locationRow.style.display = '';
                    } else {
                        locationRow.style.display = 'none';
                    }
                }

                // Type badge
                var typeEl = document.getElementById('eventModalType');
                if (typeEl) {
                    typeEl.className = 'badge ' + getTypeBadgeClass(props.type);
                    typeEl.textContent = config.labels['type_' + props.type] || '';
                }

                // Status badge
                var statusEl = document.getElementById('eventModalStatus');
                if (statusEl) {
                    statusEl.className = 'badge ' + getStatusBadgeClass(props.status);
                    statusEl.textContent = config.labels['status_' + props.status] || '';
                }

                // Admin links
                var viewLink = document.getElementById('eventModalViewLink');
                var editLink = document.getElementById('eventModalEditLink');
                if (viewLink) {
                    if (config.isAdmin && config.detailsBaseUrl) {
                        viewLink.href = config.detailsBaseUrl + '?id=' + evt.id;
                        viewLink.style.display = '';
                    } else {
                        viewLink.style.display = 'none';
                    }
                }
                if (editLink) {
                    if (config.isAdmin && config.editBaseUrl) {
                        editLink.href = config.editBaseUrl + '?id=' + evt.id;
                        editLink.style.display = '';
                    } else {
                        editLink.style.display = 'none';
                    }
                }

                // Show modal
                var bsModal = bootstrap.Modal.getOrCreateInstance(modal);
                bsModal.show();
            },

            dateClick: function (info) {
                if (config.isAdmin && config.createUrl) {
                    window.location.href = config.createUrl + '?date=' + info.dateStr;
                } else {
                    calendar.changeView('timeGridDay', info.dateStr);
                }
            },

            eventDidMount: function (info) {
                if (info.event.extendedProps.studentName) {
                    info.el.title = info.event.title + ' \u2013 ' + info.event.extendedProps.studentName;
                }
            }
        });

        calendar.render();

        // Responsive resize handler
        var resizeTimer;
        window.addEventListener('resize', function () {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(function () {
                var mobile = isMobile();
                calendar.setOption('headerToolbar', mobile ? mobileToolbar() : desktopToolbar());
            }, 250);
        });

        return calendar;
    }

    return { init: init };
})();
