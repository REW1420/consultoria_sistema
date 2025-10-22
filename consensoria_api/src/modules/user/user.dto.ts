import {
  IsString,
  IsEmail,
  IsInt,
  IsBoolean,
  IsOptional,
} from "class-validator";

export class CreateUserDto {
  @IsString()
  username!: string;

  @IsString()
  full_name!: string;

  @IsEmail()
  email!: string;

  @IsString()
  password!: string;

  @IsOptional()
  role_id!: {
    id: number;
    name?: string;
  };

  @IsOptional()
  @IsBoolean()
  active?: boolean;
}
