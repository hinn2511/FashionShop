export class User {
    id: number;
    username: string;
    password: string;
    firstName: string;
    lastName: string;
    jwtToken?: string;
    roles: string[];
}

export class Account {
    dateOfBirth: Date;
    firstName: string;
    lastName: string;
    gender: string;
    phoneNumber: string;
    email: string;
}

export class RegisterAccount extends Account {
    username: string;
    password: string;

}