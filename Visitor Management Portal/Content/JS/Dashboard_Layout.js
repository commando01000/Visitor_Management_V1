document.addEventListener('DOMContentLoaded', () => {
    const collapseBtn = document.getElementById('collapseBtn');
    const sidebar = document.getElementById('sidebar');
    const navbar = document.getElementById('navbar');
    const content = document.getElementById('content');
    const Logout_btn = document.getElementById('logout-btn');
    
    
    const sidebar_logo = document.querySelector(".sidebar-logo")
    const sidebar_profile_pic = document.querySelector(".profile-section-sidebar")

    const sidebarSettingsItems = document.querySelectorAll('.settings-items li');


    //function fetchUserInfo() {
    //    fetch('/Account/UserInfo')
    //        .then(response => {
    //            return response.text(); 
    //        })
    //        .then(text => {
    //            try {
    //                const data = JSON.parse(text);
    //                if (data.success) {
    //                    const nameElement = document.querySelector('.name');
    //                    const titleElement = document.querySelector('.title');

    //                    if (nameElement && titleElement) {
    //                        nameElement.textContent = data.name;
    //                        titleElement.textContent = data.jobTitle;
    //                    }
    //                } else {
    //                    console.error(data.message);
    //                }
    //            } catch (error) {
    //                console.error('Error parsing JSON:', error); 
    //                console.error('Response Text:', text); 
    //            }
    //        })
    //        .catch(error => {
    //            console.error('Error fetching user info:', error);
    //        });
    //}

    //fetchUserInfo();




    collapseBtn.addEventListener('click', () => {
        // Toggle the collapsed state of the sidebar and other elements
        sidebar.classList.toggle('collapsed');
        navbar.classList.toggle('collapsed');
        content.classList.toggle('collapsed');
        sidebar_logo.classList.toggle("d-none");
        sidebar_profile_pic.classList.toggle("d-none");
        document.querySelector('.collapse-btn-border').classList.toggle('collapsed');
        // Get all sidebar items

        if (sidebar.classList.contains('collapsed')) {
            sidebarSettingsItems.forEach(item => {
                item.textContent = item.getAttribute('data-short-text'); 
            });
        } else {
            sidebarSettingsItems.forEach(item => {
                item.textContent = item.getAttribute('data-full-text'); 
            });
        }
    });


    // Setting Organizaiton navigaiton 
    sidebarSettingsItems.forEach(item => {
        const path = item.getAttribute('data-path');
        item.addEventListener('click', () => {
            window.location.href = path;
        });
    })
    function checkScreenSize() {

        if (window.innerWidth <= 991) {
            // Collapse the sidebar
            sidebar.classList.add('collapsed');
            navbar.classList.add('collapsed');
            content.classList.add('collapsed');
            sidebar_logo.classList.add("d-none");
            sidebar_profile_pic.classList.add("d-none");
            document.querySelector('.collapse-btn-border').classList.toggle('collapsed');

            // Change to short labels
            sidebarSettingsItems.forEach(item => {
                item.textContent = item.getAttribute('data-short-text'); 
            });
        } else {
            // Expand the sidebar
            sidebar.classList.remove('collapsed');
            navbar.classList.remove('collapsed');
            content.classList.remove('collapsed');
            sidebar_logo.classList.remove("d-none");
            sidebar_profile_pic.classList.remove("d-none");

            // Restore full text
            sidebarSettingsItems.forEach(item => {
                item.textContent = item.getAttribute('data-full-text');
            });
        }
    }

    checkScreenSize();


    window.addEventListener('resize', checkScreenSize);


//Logout_btn.addEventListener('click', () => {
//        window.location.href = "/Account/Logout";
//    });

    const currentPath = window.location.pathname;
    const menuItems = document.querySelectorAll('.sidebar-menu li');

    //menuItems.forEach(item => {
    //    const link = item.querySelector('a');
            
    //    if (link && link.getAttribute('href') === currentPath) {
    //        item.classList.add('active-sidebar-menu');
    //    } else if (currentPath.includes('Organization') && item.classList.contains('dropdown')) {
    //        item.classList.add('active-sidebar-menu');
    //    } else {
    //        item.classList.remove('active-sidebar-menu');
    //    }
    //});


    if (sidebar_profile_pic) {
        sidebar_profile_pic.addEventListener('click', () => {
            window.location.href = "/Profile/Index";
        });
    }

    const dropdown = document.querySelector('.dropdown');
    const toggle = dropdown.querySelector('.dropdown-toggle');
    const listItems = dropdown.querySelectorAll('.settings-items ul li');

    listItems.forEach(item => {
        const link = item.getAttribute('data-path'); 
        if (link && link === currentPath) {
            dropdown.classList.add('open'); 
            item.classList.add('active');  
        } else {
            item.classList.remove('active');
        }
    });

    toggle.addEventListener('click', () => {
        dropdown.classList.toggle('open');
    });

    listItems.forEach((item, index) => {
        if (window.location.pathname === '/OrganizationDate/DataverseConnection' && index === 0) {
            item.classList.add('active');
            dropdown.classList.add('open');
            item.classList.add('active');  
        }
     
        if ((window.location.pathname === '/OrganizationUsers/InviteUsers' || window.location.pathname === '/OrganizationUsers/EditUser' || window.location.pathname === '/OrganizationUsers/UserDetails') && index === 1) {
            item.classList.add('active');
            dropdown.classList.add('open');
            item.classList.add('active');  
        }
        if ((window.location.pathname === '/OrganizationSetup/AddNewBuilding' || window.location.pathname === '/OrganizationSetup/BuildingDetails') && index === 2) {
            item.classList.add('active');
            dropdown.classList.add('open');
            item.classList.add('active');
        }
        // Add event listener to handle clicks
        item.addEventListener('click', (event) => {
            listItems.forEach(li => li.classList.remove('active'));
            event.currentTarget.classList.add('active');
        });
    });


    document.addEventListener('click', (event) => {
        if (!dropdown.contains(event.target) && dropdown.classList.contains('open')) {
            event.stopPropagation();
        }
    });


    //document.getElementById("notificationBell").addEventListener("click", function () {
    //    var notificationBox = document.getElementById("notificationBox");
    //    notificationBox.classList.toggle("show");
    //});

    document.getElementById("close-ico").addEventListener("click", function () {
        var notificationBox = document.getElementById("notificationBox");
        notificationBox.classList.toggle("show");
    });

    // Optional: Close notifications when clicking outside
    document.addEventListener("click", function (event) {
        var notificationBox = document.getElementById("notificationBox");
        var bellIcon = document.getElementById("notificationBell");

        if (!notificationBox.contains(event.target) && event.target !== bellIcon) {
            notificationBox.classList.remove("show");
        }
    });


});