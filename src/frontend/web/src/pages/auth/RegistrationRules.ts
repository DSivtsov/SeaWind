
export function buildPasswordRules(password: string, confirmPassword: string): RuleCheck[] {
    const minLen = 6;

    const hasUpper = /[A-Z]/.test(password);
    const hasLower = /[a-z]/.test(password);
    const hasDigit = /\d/.test(password);
    const hasMinLen = password.length >= minLen;
    const matches = password.length > 0 && password === confirmPassword;

    return [
        { label: `Password length ≥ ${minLen}`, ok: hasMinLen },
        { label: "Uppercase letter", ok: hasUpper },
        { label: "Lowercase letter", ok: hasLower },
        { label: "Digit", ok: hasDigit },
        { label: "Passwords match", ok: matches },
    ];
}
export type RuleCheck = {
    label: string;
    ok: boolean;
};

export function isValidEmail(value: string) {
    // MVP-level email check (avoid heavy regex)
    return value.includes("@") && value.includes(".");
}


