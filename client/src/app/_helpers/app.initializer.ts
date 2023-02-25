import { SettingService } from './../_services/setting.service';
import { AuthenticationService } from "../_services/authentication.service";


export function appInitializer(authenticationService: AuthenticationService) {
    return () => new Promise(resolve => {
        authenticationService.refreshToken()
            .subscribe()
            .add(resolve);
    });
}

export function settingInitializer(settingService: SettingService) {
    return () => new Promise(resolve => {
        settingService.getSettings()
            .subscribe()
            .add(resolve);
    });
}