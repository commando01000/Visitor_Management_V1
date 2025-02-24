document.addEventListener('DOMContentLoaded', () => {

    const backBtn = document.querySelector(".back-btn");
    const microsoft_app_setup = document.querySelector(".microsoft_app_setup");

    const saveChangesBtn = document.querySelector('.save-changes-btn');
    const organizationName = document.getElementById('Organization-Name').value;
    const organizationDomain = document.getElementById('Organization-Domain').value;
    const emailAddress = document.getElementById('Email-Address').value;
    const organizationWebsite = document.getElementById('Organization-Website').value;
    const organizationPhone = document.getElementById('Organization-Phone').value;
    const organizationURL = document.getElementById('Organization-URL') ? document.getElementById('Organization-URL').value : '';
    const clientID = document.getElementById('Client-ID') ? document.getElementById('Client-ID').value : '';
    const clientSecret = document.getElementById('Client-Secret') ? document.getElementById('Client-Secret').value : '';




    backBtn.addEventListener('click', () => {
        window.history.back();
    });

    microsoft_app_setup.addEventListener('click', () => {
        window.location.href = "/OrganizationDate/DataverseConnection";
    });

    const dataverseSwitch = document.getElementById('dataverseSwitch');
    const microsoftAppSetupBtn = document.getElementById('microsoftAppSetupBtn');
    const formGrid = document.getElementById('formGrid');


    if (dataverseSwitch && dataverseSwitch.checked) {
        formGrid.classList.remove('d-none');
        microsoftAppSetupBtn.classList.remove('d-none');
    } else {
        formGrid.classList.add('d-none');
        microsoftAppSetupBtn.classList.add('d-none');
    }

    dataverseSwitch.addEventListener('change', function () {
        if (dataverseSwitch.checked) {
            formGrid.classList.remove('d-none');
            microsoftAppSetupBtn.classList.remove('d-none');
        } else {
            formGrid.classList.add('d-none');
            microsoftAppSetupBtn.classList.add('d-none');
        }
    });

    saveChangesBtn.addEventListener('click', () => {
    $(document).trigger('ajaxStart');



        const organizationData = {
        OrganizationId: document.getElementById('organizationID').value,
        OrganizationName: document.getElementById('Organization-Name').value,
        OrganizationDomain: document.getElementById('Organization-Domain').value,
        EmailAddress: document.getElementById('Email-Address').value,
        OrganizationWebsite: document.getElementById('Organization-Website').value,
        OrganizationPhone: document.getElementById('Organization-Phone').value,
        OrganizationURL: document.getElementById('Organization-URL') ? document.getElementById('Organization-URL').value : '',
        ClientID: document.getElementById('Client-ID') ? document.getElementById('Client-ID').value : '',
        ClientSecret: document.getElementById('Client-Secret') ? document.getElementById('Client-Secret').value : '',
            TenantID: document.getElementById('TenantID') ? document.getElementById('TenantID').value : '',
            WebApi: document.getElementById('WebApi') ? document.getElementById('WebApi').value : '',
        EnableDataverseConnection: document.getElementById('dataverseSwitch').checked
    };

    console.log(organizationData);

        fetch('/OrganizationDate/updateOrganizationDate', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(organizationData)
    })
        .then(response => response.json())  
        .then(data => {
            if (data.success) {
                Notify(data.message, NotifyStatus.Success);

            } else {
                Notify(data.message, NotifyStatus.Error);
            }
        })
        .catch(error => {
            console.error('Error saving data:', error);

            Notify('An error occurred while saving the data.', NotifyStatus.Error);
        })
        .finally(() => {
            $(document).trigger('ajaxStop');
        });
});

});
